using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Application.Ticketing.DTOs;
using TEDx.Domain.Common;
using TEDx.Domain.Ticketing.Entities;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Application.Ticketing.Command.CancelEvent
{
    public sealed class CancelEventCommandHandler(
        IAppDbContext dbContext,
        IClock clock,
        ICurrentUser currentUser,
        ILogger<CancelEventCommandHandler> logger
        ) : IRequestHandler<CancelEventCommand, Result<CancelEventResponse>>
    {
        public async Task<Result<CancelEventResponse>> Handle(CancelEventCommand request, CancellationToken cancellationToken)
        {
            var eventEntity = await dbContext.Events
                .Include(x => x.Orders)!
                    .ThenInclude(o => o.Tickets)
                .Include(x => x.Tickets)
                .FirstOrDefaultAsync(x => x.Id == request.id, cancellationToken);

            if (eventEntity is null)
            {
                return Result<CancelEventResponse>.Failure(Errors_Common.NotFound);
            }

            if (eventEntity.Status != EventStatus.Published && eventEntity.Status != EventStatus.Archived)
            {
                return Result<CancelEventResponse>.Failure(Errors_Common.IllegalStatusTransition);
            }

            int voidedTickets = 0;
            int checkedInTicketsRetained = 0;
            int releasedHolds = 0;
            int refundEntriesRecorded = 0;
            var now = clock.UtcNow;
            var actorId = currentUser.UserId?.ToString() ?? currentUser.Email ?? "System";

            await using var transaction = await dbContext.BeginTransactionAsync(cancellationToken);
            try
            {
                eventEntity.Cancel();

                if (eventEntity.Tickets is not null)
                {
                    foreach (var ticket in eventEntity.Tickets)
                    {
                        if (ticket.Status == TicketStatus.Issued)
                        {
                            ticket.Void();
                            voidedTickets++;
                        }
                        else if (ticket.Status == TicketStatus.CheckedIn)
                        {
                            checkedInTicketsRetained++;
                        }
                    }
                }

                if (eventEntity.Orders is not null)
                {
                    foreach (var order in eventEntity.Orders)
                    {
                        if (order.Status == OrderStatus.PendingPayment)
                        {
                            order.Cancel(now);
                            releasedHolds += order.Quantity;
                        }
                        else if (order.Status == OrderStatus.Paid)
                        {
                            var refundEntry = new RefundEntry
                            {
                                Id = Guid.NewGuid(),
                                OrderId = order.Id,
                                Reason = $"Event cancellation: {eventEntity.TitleEn ?? eventEntity.Id.ToString()}",
                                VoidedTicketsCount = order.Tickets?.Count(x => x.Status == TicketStatus.Voided) ?? 0,
                                CheckedInTicketsRetained = order.Tickets?.Count(x => x.Status == TicketStatus.CheckedIn) ?? 0,
                                SeatsReleased = order.Quantity,
                                RefundedBy = actorId,
                                Order = order
                            };

                            dbContext.RefundEntries.Add(refundEntry);
                            refundEntriesRecorded++;
                        }
                    }
                }

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }

            logger.LogInformation(
                "Event {EventId} was CANCELLED by Admin {ActorId}. Ripple counts: VoidedTickets={VoidedTickets}, CheckedInRetained={CheckedInRetained}, ReleasedHolds={ReleasedHolds}, RefundEntries={RefundEntries}",
                eventEntity.Id,
                actorId,
                voidedTickets,
                checkedInTicketsRetained,
                releasedHolds,
                refundEntriesRecorded);

            var response = new CancelEventResponse(
                eventId: eventEntity.Id,
                status: eventEntity.Status,
                voidedTickets: voidedTickets,
                checkedInTicketsRetained: checkedInTicketsRetained,
                releasedHolds: releasedHolds,
                refundEntriesRecorded: refundEntriesRecorded);

            return Result<CancelEventResponse>.Success(response);
        }
    }
}

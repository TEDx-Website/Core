using System;
using System.Collections.Generic;
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
            await using var transaction = await dbContext.BeginTransactionAsync(cancellationToken);

            // CONTRACT: the reserve/hold handler MUST take the same
            // (UPDLOCK, HOLDLOCK) on the event row — and re-check Status — before
            // inserting a PendingPayment order, otherwise this invariant is lost.
            var eventEntity = await dbContext.Events
                .FromSqlInterpolated(
                    $"SELECT * FROM Events WITH (UPDLOCK, HOLDLOCK) WHERE Id = {request.id}")
                .Include(x => x.Orders!)
                    .ThenInclude(o => o.Tickets)
                .Include(x => x.Tickets)
                .AsSplitQuery()
                .FirstOrDefaultAsync(cancellationToken);

            if (eventEntity is null)
            {
                return Result<CancelEventResponse>.Failure(Errors_Common.NotFound);
            }

            if (eventEntity.Status is not (EventStatus.Published or EventStatus.Archived))
            {
                return Result<CancelEventResponse>.Failure(Errors_Common.IllegalStatusTransition);
            }

            var now = clock.UtcNow;
            var actorId = currentUser.UserId?.ToString() ?? currentUser.Email ?? "System";

            eventEntity.Cancel();

            var (voidedTickets, checkedInTicketsRetained) = VoidIssuedTickets(eventEntity);
            var releasedHolds = CancelPendingOrders(eventEntity, now);
            var refundEntriesRecorded = RecordRefundsForPaidOrders(eventEntity, actorId);

            try
            {
                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<CancelEventResponse>.Failure(Errors_Common.ConcurrencyConflict);
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

        private static (int Voided, int CheckedInRetained) VoidIssuedTickets(Event eventEntity)
        {
            var voided = 0;
            var checkedInRetained = 0;

            foreach (var ticket in eventEntity.Tickets ?? Enumerable.Empty<Ticket>())
            {
                switch (ticket.Status)
                {
                    case TicketStatus.Issued:
                        ticket.Void();
                        voided++;
                        break;
                    case TicketStatus.CheckedIn:
                        checkedInRetained++;
                        break;
                }
            }

            return (voided, checkedInRetained);
        }

        private static int CancelPendingOrders(Event eventEntity, DateTime now)
        {
            var releasedHolds = 0;

            foreach (var order in eventEntity.Orders ?? Enumerable.Empty<Order>())
            {
                if (order.Status == OrderStatus.PendingPayment)
                {
                    order.Cancel(now);
                    releasedHolds += order.Quantity;
                }
            }

            return releasedHolds;
        }

        private int RecordRefundsForPaidOrders(Event eventEntity, string actorId)
        {
            var recorded = 0;

            foreach (var order in eventEntity.Orders ?? Enumerable.Empty<Order>())
            {
                if (order.Status != OrderStatus.Paid)
                {
                    continue;
                }

                dbContext.RefundEntries.Add(BuildRefundEntry(eventEntity, order, actorId));
                recorded++;
            }

            return recorded;
        }

        private static RefundEntry BuildRefundEntry(Event eventEntity, Order order, string actorId)
        {
            var checkedIn = order.Tickets?.Count(t => t.Status == TicketStatus.CheckedIn) ?? 0;
            var voided = order.Tickets?.Count(t => t.Status == TicketStatus.Voided) ?? 0;


            return new RefundEntry
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Amount = CalculateAmountOwed(order.TotalSnapshot, order.Quantity, checkedIn),
                Reason = $"Event cancellation: {eventEntity.TitleEn ?? eventEntity.Id.ToString()}",
                VoidedTicketsCount = voided,
                CheckedInTicketsRetained = checkedIn,
                SeatsReleased = voided,
                RefundedBy = actorId,
                Order = order
            };
        }

        private static decimal CalculateAmountOwed(decimal orderTotal, int quantity, int checkedInCount)
        {
            if (quantity <= 0 || checkedInCount <= 0)
            {
                return orderTotal;
            }

            if (checkedInCount >= quantity)
            {
                return 0m;
            }

            var consumed = Math.Round(
                orderTotal * checkedInCount / quantity,
                2,
                MidpointRounding.AwayFromZero);

            return orderTotal - consumed;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Application.Ticketing.Availability;
using TEDx.Application.Ticketing.DTOs;
using TEDx.Domain.Common;
using TEDx.Domain.Common.Exceptions;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Application.Ticketing.Command.ChangeEventStatus
{
    public sealed class ChangeEventStatusCommandHandler : IRequestHandler<ChangeEventStatusCommand, Result<ChangeEventStatusDTO>>
    {
        private readonly IAppDbContext _context;

        public ChangeEventStatusCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ChangeEventStatusDTO>> Handle(
            ChangeEventStatusCommand request,
            CancellationToken cancellationToken)
        {
            await using var transaction = await _context.BeginTransactionAsync(cancellationToken);

            var @event = await _context.Events
                .FromSqlInterpolated(
                    $"SELECT * FROM Events WITH (UPDLOCK, HOLDLOCK) WHERE Id = {request.Id}")
                .FirstOrDefaultAsync(cancellationToken);

            if (@event is null)
                return Result<ChangeEventStatusDTO>.Failure(
                    Errors_Common.NotFound);

            try
            {
                switch (request.TargetStatus)
                {
                    case EventStatus.Published:
                        @event.Publish();
                        break;

                    case EventStatus.Draft:
                        var liveOrderCount = await _context.Orders
                            .CountAsync(
                                o => o.EventId == @event.Id
                                     && (o.Status == OrderStatus.PendingPayment
                                         || o.Status == OrderStatus.Paid),
                                cancellationToken);
                        @event.Revert(liveOrderCount);
                        break;

                    case EventStatus.Archived:
                        @event.Archive();
                        break;

                    default:
                        // The validator already rejects every other target (Cancelled → 422).
                        // Reaching here means that guard was bypassed — fail loudly rather
                        // than returning 200 with the status untouched.
                        return Result<ChangeEventStatusDTO>.Failure(
                            Errors_Common.ValidationError);
                }
            }
            catch (EventHasOrdersException)
            {
                return Result<ChangeEventStatusDTO>.Failure(
                    Errors_Ticketing.HasOrdersCannotUnpublish);
            }
            catch (EventNotPublishableException ex)
            {
                return Result<ChangeEventStatusDTO>.Failure(ex.Block switch
                {
                    EventPublishBlock.InvalidCapacity => Errors_Ticketing.InvalidCapacity,
                    _ => Errors_Ticketing.InvalidTicketPrice
                });
            }
            catch (InvalidStateTransitionException)
            {
                return Result<ChangeEventStatusDTO>.Failure(
                    Errors_Common.IllegalStatusTransition);
            }

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<ChangeEventStatusDTO>.Failure(
                    Errors_Common.ConcurrencyConflict);
            }

            return Result<ChangeEventStatusDTO>.Success(
                new ChangeEventStatusDTO(
                    @event.Status,
                    Convert.ToBase64String(@event.RowVersion)));
        }
    }
}

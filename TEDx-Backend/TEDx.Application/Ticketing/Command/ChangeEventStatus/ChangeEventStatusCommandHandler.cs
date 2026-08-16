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
            var @event = await _context.Events.FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

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
                        var orderCount = await _context.Orders
                            .CountAsync(
                                o => o.EventId == @event.Id,
                                cancellationToken);
                        @event.Revert(orderCount);
                        break;

                    case EventStatus.Archived:
                        @event.Archive();
                        break;
                }
            }
            catch (EventHasOrdersException)
            {
                return Result<ChangeEventStatusDTO>.Failure(
                    Errors_Ticketing.HasOrdersCannotUnpublish);
            }
            catch (InvalidStateTransitionException)
            {
                return Result<ChangeEventStatusDTO>.Failure(
                    Errors_Common.IllegalStatusTransition);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result<ChangeEventStatusDTO>.Success(
                new ChangeEventStatusDTO { Status = @event.Status, RowVersion = @event.RowVersion });
        }
    }
}

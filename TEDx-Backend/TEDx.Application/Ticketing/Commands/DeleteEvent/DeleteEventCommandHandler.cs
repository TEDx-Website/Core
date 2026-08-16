using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;
using TEDx.Application.Common.Errors;
using TEDx.Domain.Ticketing.Enums;
using Microsoft.EntityFrameworkCore;

namespace TEDx.Application.Ticketing.Commands.DeleteEvent
{
    public sealed class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, Result<Unit>>
    {
        private readonly IApplicationDbContext _appDbContext;
        private readonly IClock _clock;

        public DeleteEventCommandHandler(IApplicationDbContext appDbContext, IClock clock)
        {
            _appDbContext = appDbContext;
            _clock = clock;
        }

        public async Task<Result<Unit>> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
        {
            var entity = await _appDbContext.Events.FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken);
            if (entity == null)
            {
                return Result<Unit>.Failure(CommonErrors.NotFound);
            }
            var HasOrders = await _appDbContext.Orders.AnyAsync(o => o.EventId == request.EventId, cancellationToken);

            if (HasOrders)
            {
                return Result<Unit>.Failure(TicketingErrors.EventHasOrders);
            }
            entity.IsDeleted = true;
            entity.DeletedAtUtc = _clock.UtcNow;

            await _appDbContext.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}

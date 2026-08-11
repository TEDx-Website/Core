using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;
using TEDx.Application.Common.Errors;
using TEDx.Domain.Ticketing.Enums;
using Microsoft.EntityFrameworkCore;

namespace TEDx.Application.Ticketing.Command.DeleteEvent
{
    internal class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, Result<Unit>>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IClock _clock;

        public DeleteEventCommandHandler(IAppDbContext appDbContext, IClock clock)
        {
            _appDbContext = appDbContext;
            _clock = clock;
        }

        public async Task<Result<Unit>> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
        {
            var entity = await _appDbContext.Events.FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken);
            if (entity == null)
            {
                return Result<Unit>.Failure(Errors_Common.NotFound);
            }
            var HasOrders = await _appDbContext.Orders.AnyAsync(o => o.EventId == request.EventId, cancellationToken);

            if (HasOrders)
            {
                return Result<Unit>.Failure(Errors_Ticketing.EventHasOrders);
            }
            entity.IsDeleted = true;
            entity.DeletedAtUtc = _clock.UtcNow;

            await _appDbContext.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}

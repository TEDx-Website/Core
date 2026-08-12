using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Command.UpdateEvent
{
    public sealed class UpdateEventCommandHandler(IAppDbContext dbContext) : IRequestHandler<UpdateEventCommand, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(UpdateEventCommand request, CancellationToken ct)
        {
            var eventEntity = dbContext.Events.FirstOrDefault(x => x.Id == request.EventId);
            if (eventEntity == null)
            {
                // event isnt exist
            }

            eventEntity.TitleEn = request.TitleEn;
            eventEntity.TitleAr = request.TitleAr;
            eventEntity.DescriptionEn = request.DescriptionEn;
            eventEntity.DescriptionAr = request.DescriptionAr;
            eventEntity.Venue = request.Venue;
            eventEntity.StartAtUtc = eventEntity.StartAtUtc;
            eventEntity.EndAtUtc = eventEntity.EndAtUtc;
            eventEntity.Capacity = request.Capacity;
            eventEntity.TicketPrice = request.TicketPrice;
            eventEntity.MaxIndividualQtyPerOrder = request.MaxIndividualQtyPerOrder;
            eventEntity.RowVersion = request.RowVersion;

            await dbContext.SaveChangesAsync();
        }
    }
}

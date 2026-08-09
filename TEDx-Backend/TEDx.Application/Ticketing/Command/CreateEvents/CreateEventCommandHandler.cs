using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Application.Common.Interfaces;
using TEDx.Application.Ticketing.DTOs;
using TEDx.Domain.Common;
using TEDx.Domain.Ticketing.Entities;

namespace TEDx.Application.Ticketing.Command.CreateEvents
{
    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Result<CreateEventDTO>>
    {
        private readonly IAppDbContext _appDbContext;
        public CreateEventCommandHandler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<Result<CreateEventDTO>> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            var eventEntity = Event.Create(
            titleEn: request.TitleEn,
            titleAr: request.TitleAr,
            descriptionEn: request.DescriptionEn,
            descriptionAr: request.DescriptionAr,
            startAtUtc: request.StartsAtUtc,
            endAtUtc: request.EndsAtUtc,
            venue: request.Location,
            capacity: request.Capacity,
            ticketPrice: request.TicketPrice.Amount,
            maxIndividualQtyPerOrder: request.MaxIndividualQtyPerOrder ?? 0,
            imageUrl: request.ImageUrl);

            await _appDbContext.Events.AddAsync(eventEntity, cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);
            var result = new CreateEventDTO
            {
                Id = eventEntity.Id,
                TitleEn = eventEntity.TitleEn!,
                TitleAr = eventEntity.TitleAr!,
                DescriptionEn = eventEntity.DescriptionEn!,
                DescriptionAr = eventEntity.DescriptionAr!,
                StartsAtUtc = eventEntity.StartAtUtc,
                EndsAtUtc = eventEntity.EndAtUtc,
                Location = eventEntity.Venue!,
                Capacity = eventEntity.Capacity,
                TicketPrice = new MoneyDto(eventEntity.TicketPrice,"EGP"),
                MaxIndividualQtyPerOrder = eventEntity.MaxIndividualQtyPerOrder,
                ImageUrl = eventEntity.ImageUrl,
                Status = eventEntity.Status,
                RowVersion = eventEntity.RowVersion
            };
            return Result<CreateEventDTO>.Success(result);
        }
    }
}

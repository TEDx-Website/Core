using TEDx.Application.Common.Dtos;
using TEDx.Application.Common.Constants;
using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Application.Common.Interfaces;
using TEDx.Application.Ticketing.Dtos;
using TEDx.Domain.Common;
using TEDx.Domain.Ticketing.Entities;

namespace TEDx.Application.Ticketing.Commands.CreateEvent
{
    public sealed class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Result<CreateEventResponse>>
    {
        private readonly IApplicationDbContext _appDbContext;
        public CreateEventCommandHandler(IApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<Result<CreateEventResponse>> Handle(CreateEventCommand request, CancellationToken cancellationToken)
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
            maxIndividualQtyPerOrder: request.MaxIndividualQtyPerOrder,
            imageUrl: request.ImageUrl);

            await _appDbContext.Events.AddAsync(eventEntity, cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);
            var result = new CreateEventResponse(
                Id: eventEntity.Id,
                TitleEn: eventEntity.TitleEn!,
                TitleAr: eventEntity.TitleAr!,
                DescriptionEn: eventEntity.DescriptionEn!,
                DescriptionAr: eventEntity.DescriptionAr!,
                StartsAtUtc: eventEntity.StartAtUtc,
                EndsAtUtc: eventEntity.EndAtUtc,
                Location: eventEntity.Venue!,
                Capacity: eventEntity.Capacity,
                TicketPrice: new MoneyDto(eventEntity.TicketPrice, CurrencyCodes.Egp),
                MaxIndividualQtyPerOrder: eventEntity.MaxIndividualQtyPerOrder,
                ImageUrl: eventEntity.ImageUrl,
                Status: eventEntity.Status,
                RowVersion: Convert.ToBase64String(eventEntity.RowVersion));
            return Result<CreateEventResponse>.Success(result);
        }
    }
}

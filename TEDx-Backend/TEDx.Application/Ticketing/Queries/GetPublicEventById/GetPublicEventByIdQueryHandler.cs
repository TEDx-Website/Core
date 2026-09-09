using MediatR;
using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.Constants;
using TEDx.Application.Common.Dtos;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Application.Ticketing.Services;
using TEDx.Domain.Common;
using TEDx.Domain.Ticketing.Entities;
using TEDx.Domain.Ticketing.Enums;
namespace TEDx.Application.Ticketing.Queries.GetPublicEventById
{
    public class GetPublicEventByIdQueryHandler : IRequestHandler<GetPublicEventByIdQuery, Result<GetPublicEventByIdResponse>>
    {
        private readonly IApplicationDbContext context;
        private readonly IEventSeatAvailabilityReader eventSeatAvailabilityReader;
        public GetPublicEventByIdQueryHandler(IApplicationDbContext _Context , IEventSeatAvailabilityReader eventSeat)
        {
            context = _Context;
            eventSeatAvailabilityReader = eventSeat;
        }
        public async Task<Result<GetPublicEventByIdResponse>> Handle(GetPublicEventByIdQuery request, CancellationToken ct)
        {
            var @Event = await context.Events.AsNoTracking()
                        .Include(e => e.Packages)
                        .Where(e => e.Id == request.Id && e.Status == EventStatus.Published && !e.IsDeleted)
                        .FirstOrDefaultAsync(ct);

            if(@Event is null)
            {
                return Result<GetPublicEventByIdResponse>.Failure(CommonErrors.NotFound);
            }
            var availability = await eventSeatAvailabilityReader.GetAsync(
                        @Event.Id,
                        ct);

            var Result = new GetPublicEventByIdResponse(
                @Event.Id,
                @Event.TitleEn!,
                @Event.TitleAr!,
                @Event.DescriptionEn!,
                @Event.DescriptionAr!,
                DateTime.SpecifyKind(
                    @Event.StartAtUtc,
                    DateTimeKind.Utc),
                DateTime.SpecifyKind(
                    @Event.EndAtUtc,
                    DateTimeKind.Utc),
                @Event.Venue!,
                @Event.ImageUrl,
                @Event.Capacity,
                availability?.RemainingSeats ?? 0,
                Event.Status.ToString(),
                new MoneyDto(@Event.TicketPrice, CurrencyCodes.Egp),
                @Event.MaxIndividualQtyPerOrder,
                @Event.Packages!
                    .Where(p => p.IsActive && !p.IsDeleted)
                    .Select(p => new PackageResponse(
                        p.Id,
                        p.NameEn!,
                        p.NameAr!,
                        p.SeatsPerPackage,
                        p.MaxQuantityPerOrder,
                        new MoneyDto(p.Price, CurrencyCodes.Egp),
                        p.IsActive))
                    .ToList()
                );

            return Result<GetPublicEventByIdResponse>.Success(Result);
        }

    }
}

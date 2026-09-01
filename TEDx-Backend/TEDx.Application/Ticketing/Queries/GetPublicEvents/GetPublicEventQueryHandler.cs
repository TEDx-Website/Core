using System.Net.NetworkInformation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.Constants;
using TEDx.Application.Common.Dtos;
using TEDx.Application.Common.Interfaces;
using TEDx.Application.Common.Pagination;
using TEDx.Application.Ticketing.Services;
using TEDx.Domain.Common;
using TEDx.Domain.Ticketing.Entities;
using TEDx.Domain.Ticketing.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TEDx.Application.Ticketing.Queries.GetPublicEvents
{
    public class GetPublicEventQueryHandler : IRequestHandler<GetPublicEventsQuery, Result<PagedResult<GetPublicEventResponse>>>
    {
        private readonly IApplicationDbContext dbContext;
        private readonly IClock clock;
        private readonly IEventSeatAvailabilityReader eventSeat;
        public GetPublicEventQueryHandler(IApplicationDbContext _context, IClock _clock, IEventSeatAvailabilityReader reader)
        {
            dbContext = _context;
            clock = _clock;
            eventSeat = reader;
        }
        public async Task<Result<PagedResult<GetPublicEventResponse>>> Handle(GetPublicEventsQuery request, CancellationToken cancellationToken)
        {
            var now = clock.UtcNow;
            var Query = dbContext.Events.AsNoTracking().Where(s => s.Status == EventStatus.Published);
            // when
            Query = request.When?.Trim().ToLowerInvariant() switch
            {
                "past" => Query.Where(e => e.StartAtUtc < now),

                _ => Query.Where(e => e.StartAtUtc >= now)
            };
            // sorting
            var ordered = EventSorting.Public.Apply(Query,request.Sort);

            if (ordered.IsError)
            {
                return Result<PagedResult<GetPublicEventResponse>>.Failure(ordered.Errors);
            }

            var page = await ordered.Value.ToPagedResultAsync(PagedRequest.From(request.Page, request.PageSize), cancellationToken);
            if (page.Items.Count == 0)
            {
                return Result<PagedResult<GetPublicEventResponse>>.Success(
                    page.Map(e => ToDto(e, 0)));
            }
            var availability = await eventSeat.GetManyAsync(
            [.. page.Items.Select(e => e.Id)],cancellationToken);

            var result = page.Map(e => ToDto(e, availability.RemainingSeatsFor(e.Id)));

            return Result<PagedResult<GetPublicEventResponse>>.Success(result);

        }

        private static GetPublicEventResponse ToDto(
        Event entity,
        int remainingSeats)
        => new(
            Id: entity.Id,
            TitleEn: entity.TitleEn!,
            TitleAr: entity.TitleAr!,
            DescriptionEn: entity.DescriptionEn!,
            DescriptionAr: entity.DescriptionAr!,
            StartsAtUtc: DateTime.SpecifyKind(
                entity.StartAtUtc,
                DateTimeKind.Utc),
            EndsAtUtc: DateTime.SpecifyKind(
                entity.EndAtUtc,
                DateTimeKind.Utc),
            Location: entity.Venue,
            ImageUrl: entity.ImageUrl,
            Capacity: entity.Capacity,
            RemainingSeats: remainingSeats,
            Status: entity.Status.ToString(),
            TicketPrice: new MoneyDto(
                entity.TicketPrice,
                CurrencyCodes.Egp),
            PriceFrom: new MoneyDto(
                entity.TicketPrice,
                CurrencyCodes.Egp));
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.DTOs;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Application.Common.Pagination;
using TEDx.Application.Ticketing.Availability;
using TEDx.Application.Ticketing.DTOs;
using TEDx.Domain.Common;
using TEDx.Domain.Ticketing.Entities;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Application.Ticketing.Queries.GetAdminEvents;

public sealed class GetAdminEventsQueryHandler(
    IAppDbContext db,
    IEventSeatAvailabilityReader seatAvailability)
    : IRequestHandler<GetAdminEventsQuery, Result<PagedResult<AdminEventListItemDTO>>>
{
    private const string StatusParameterName = "status";
    private const string Currency = "EGP";

    public async Task<Result<PagedResult<AdminEventListItemDTO>>> Handle(
        GetAdminEventsQuery request,
        CancellationToken cancellationToken)
    {
        var status = ParseStatus(request.Status);
        if (status.IsError)
        {
            return Result<PagedResult<AdminEventListItemDTO>>.Failure(status.Errors);
        }

        var filtered = ApplyFilters(db.Events.AsNoTracking(), status.Value, request.Search);

        var ordered = EventSorting.Admin.Apply(filtered, request.Sort);
        if (ordered.IsError)
        {
            return Result<PagedResult<AdminEventListItemDTO>>.Failure(ordered.Errors);
        }

        var page = await ordered.Value.ToPagedResultAsync(
            PageRequest.From(request.Page, request.PageSize),
            cancellationToken);

        return Result<PagedResult<AdminEventListItemDTO>>.Success(
            await ToDtoPageAsync(page, cancellationToken));
    }

    private static Result<EventStatus?> ParseStatus(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<EventStatus?>.Success(null);
        }

        var token = value.Trim();
        var names = Enum.GetNames<EventStatus>();
        var name = Array.Find(
            names,
            allowed => string.Equals(allowed, token, StringComparison.OrdinalIgnoreCase));

        if (name is not null)
        {
            return Result<EventStatus?>.Success(Enum.Parse<EventStatus>(name));
        }

        return Error.Validation(
            Errors_Common.ValidationError.Code,
            $"Value '{(token.Length <= 40 ? token : token[..40] + "…")}' is not supported for "
            + $"'{StatusParameterName}'. Allowed values: {string.Join(", ", names)}.",
            StatusParameterName);
    }

    private static IQueryable<Event> ApplyFilters(
        IQueryable<Event> source,
        EventStatus? status,
        string? search)
    {
        if (status is not null)
        {
            var value = status.Value;
            source = source.Where(e => e.Status == value);
        }

        var term = search?.Trim();
        if (!string.IsNullOrEmpty(term))
        {
            source = source.Where(e =>
                (e.TitleEn != null && e.TitleEn.Contains(term))
                || (e.TitleAr != null && e.TitleAr.Contains(term)));
        }

        return source;
    }

    private async Task<PagedResult<AdminEventListItemDTO>> ToDtoPageAsync(
        PagedResult<Event> page,
        CancellationToken cancellationToken)
    {
        if (page.Items.Count == 0)
        {
            // Selector never runs; the early return exists to skip the availability query.
            return page.Map(entity => ToDto(entity, remainingSeats: 0));
        }

        var availability = await seatAvailability.GetManyAsync(
            [.. page.Items.Select(entity => entity.Id)],
            cancellationToken);

        return page.Map(entity => ToDto(entity, RemainingSeatsOf(entity, availability)));
    }

    private static AdminEventListItemDTO ToDto(Event entity, int remainingSeats)
        => new(
            Id: entity.Id,
            TitleEn: entity.TitleEn,
            TitleAr: entity.TitleAr,
            StartsAtUtc: entity.StartAtUtc,
            EndsAtUtc: entity.EndAtUtc,
            Location: entity.Venue,
            Capacity: entity.Capacity,
            Status: entity.Status,
            TicketPrice: new MoneyDTO(entity.TicketPrice, Currency),
            RemainingSeats: remainingSeats,
            RowVersion: entity.RowVersion is null or { Length: 0 }
                ? string.Empty
                : Convert.ToBase64String(entity.RowVersion));

    private static int RemainingSeatsOf(
        Event entity,
        IReadOnlyDictionary<Guid, EventSeatAvailability> availability)
        => availability.TryGetValue(entity.Id, out var seats)
            ? seats.RemainingSeats
            : entity.Capacity;
}

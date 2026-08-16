using TEDx.Application.Common.Dtos;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Application.Ticketing.Dtos;

/// <summary>
/// One row of the <c>GET /api/v1/admin/events</c> list payload. The payload is the
/// list, so the element is a <c>Dto</c>, not a <c>Response</c> (Naming §0.2).
/// </summary>
public sealed record AdminEventListItemDto(
    Guid Id,
    string? TitleEn,
    string? TitleAr,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    string? Location,
    int Capacity,
    EventStatus Status,
    MoneyDto TicketPrice,
    int RemainingSeats,
    string RowVersion);

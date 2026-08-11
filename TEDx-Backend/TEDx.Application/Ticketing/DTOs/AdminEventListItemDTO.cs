using TEDx.Application.Common.DTOs;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Application.Ticketing.DTOs;

public sealed record AdminEventListItemDTO(
    Guid Id,
    string? TitleEn,
    string? TitleAr,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    string? Location,
    int Capacity,
    EventStatus Status,
    MoneyDTO TicketPrice,
    int RemainingSeats,
    string RowVersion);

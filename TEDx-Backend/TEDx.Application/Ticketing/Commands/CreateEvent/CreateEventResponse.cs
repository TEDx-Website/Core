using TEDx.Application.Common.Dtos;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Application.Ticketing.Commands.CreateEvent;

public sealed record CreateEventResponse(
    Guid Id,
    string TitleEn,
    string TitleAr,
    string DescriptionEn,
    string DescriptionAr,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    string Location,
    int Capacity,
    MoneyDto TicketPrice,
    int? MaxIndividualQtyPerOrder,
    string? ImageUrl,
    EventStatus Status,
    string RowVersion);

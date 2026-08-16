using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Application.Ticketing.Commands.ChangeEventStatus;

public sealed record ChangeEventStatusResponse(
    EventStatus Status,
    string RowVersion);

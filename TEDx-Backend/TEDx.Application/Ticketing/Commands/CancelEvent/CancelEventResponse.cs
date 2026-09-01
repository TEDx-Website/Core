using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Application.Ticketing.Commands.CancelEvent;

public sealed record CancelEventResponse(
    Guid EventId,
    EventStatus Status,
    int VoidedTickets,
    int CheckedInTicketsRetained,
    int ReleasedHolds,
    int RefundEntriesRecorded);

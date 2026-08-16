using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Application.Ticketing.DTOs
{
    public sealed record CancelEventResponse(
        Guid eventId,
        EventStatus status,
        int voidedTickets,
        int checkedInTicketsRetained,
        int releasedHolds,
        int refundEntriesRecorded
    );
}

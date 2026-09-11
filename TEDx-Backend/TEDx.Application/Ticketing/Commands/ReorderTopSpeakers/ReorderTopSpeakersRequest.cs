using System.Collections.Generic;

namespace TEDx.Application.Ticketing.Commands.ReorderTopSpeakers
{
    public sealed record ReorderTopSpeakersRequest(IReadOnlyList<TopSpeakerOrderItem> Items);
}

using System;

namespace TEDx.Application.Ticketing.Commands.ReorderTopSpeakers
{
    public sealed record TopSpeakerOrderItem(Guid SpeakerId, int OrderIndex);
}

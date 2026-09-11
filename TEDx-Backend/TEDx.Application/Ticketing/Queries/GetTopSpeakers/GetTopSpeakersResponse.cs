using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Application.Ticketing.Queries.GetTopSpeakers
{
    public sealed record TopSpeakerResponse(
        Guid Id,
        string SpeakerName,
        string SpeakerPictureUrl,
        string SpeakerRole,
        string TalkTitle,
        string TalkTrack,
        string TalkShortDescription,
        int OrderIndex);
}

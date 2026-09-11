using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Application.Ticketing.Queries.GetAdminSpeakers
{
    public sealed record AdminSpeakerResponse(
        Guid Id,
        string SpeakerName,
        string SpeakerPictureUrl,
        string SpeakerRole,
        string SpeakerBio,
        List<string?> EventTitles   
    );
}

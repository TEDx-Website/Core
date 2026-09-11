using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Application.Ticketing.Commands.UpdateSpeaker
{
    public sealed record UpdateSpeakerRequest(
    string SpeakerName,
    string SpeakerPictureUrl,
    string SpeakerRole,
    string SpeakerBio,
    string TalkTitle,
    string TalkTrack,
    string TalkShortDescription,
    string? SpeakerLinkedInUrl,
    string? SpeakerXUrl,
    string? SpeakerWebsiteUrl,
    List<Guid> EventIds);
}

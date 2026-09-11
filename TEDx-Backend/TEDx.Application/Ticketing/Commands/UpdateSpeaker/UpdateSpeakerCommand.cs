using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Commands.UpdateSpeaker
{
    public sealed record UpdateSpeakerCommand(
    Guid SpeakerId,
    string SpeakerName,
    string SpeakerPictureUrl,
    string SpeakerRole,
    string SpeakerBio,
    string TalkTitle,
    string TalkTrack,
    string TalkShortDescription,
    string? SpeakerLinkedInUrl,
    string? SpeakerXUrl,
    string? SpeakerWebsiteUrl)
    : IRequest<Result<Unit>>;
}

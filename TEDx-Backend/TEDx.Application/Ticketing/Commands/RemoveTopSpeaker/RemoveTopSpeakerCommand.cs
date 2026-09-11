using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Commands.RemoveTopSpeaker
{
    public sealed record RemoveTopSpeakerCommand(
    Guid SpeakerId)
    : IRequest<Result<Unit>>;
}

using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Commands.UpdateSpeaker
{
    public sealed record UpdateSpeakerCommand(
    Guid SpeakerId,
    string Name,
    string PictureUrl,
    string TagLine,
    string Description)
    : IRequest<Result<Unit>>;
}

using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Commands.CreateSpeaker
{
    public sealed record CreateSpeakerCommand(
    string Name,
    string PictureUrl,
    string TagLine,
    string Description)
    : IRequest<Result<Unit>>;
}

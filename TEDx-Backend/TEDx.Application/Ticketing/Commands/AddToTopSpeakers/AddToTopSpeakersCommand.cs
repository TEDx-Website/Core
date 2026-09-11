using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Commands.AddToTopSpeakers
{
    public sealed record AddTopSpeakerCommand(
     Guid SpeakerId)
     : IRequest<Result<Unit>>;
}

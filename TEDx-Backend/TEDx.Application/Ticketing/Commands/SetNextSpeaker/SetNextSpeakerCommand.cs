using MediatR;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Commands.SetNextSpeaker
{
    public sealed record SetNextSpeakerCommand(
        Guid SpeakerId)
        : IRequest<Result<Unit>>;
}

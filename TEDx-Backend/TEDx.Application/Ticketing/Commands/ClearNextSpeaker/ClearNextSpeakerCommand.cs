using MediatR;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Commands.ClearNextSpeaker
{
    public sealed record ClearNextSpeakerCommand
        : IRequest<Result<Unit>>;
}

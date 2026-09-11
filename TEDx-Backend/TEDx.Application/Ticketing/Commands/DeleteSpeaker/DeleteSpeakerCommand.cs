using MediatR;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Commands.DeleteSpeaker;

public sealed record DeleteSpeakerCommand(
    Guid SpeakerId)
    : IRequest<Result<Unit>>;

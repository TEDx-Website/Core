using MediatR;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Queries.GetNextSpeaker
{
    public sealed record GetNextSpeakerQuery
        : IRequest<Result<NextSpeakerResponse>>;
}

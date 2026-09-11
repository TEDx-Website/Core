using MediatR;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Queries.GetSpeakers
{
    public sealed record GetSpeakersQuery(
        string? Search,
        string? Category)
        : IRequest<Result<SpeakerCatalogResponse>>;
}

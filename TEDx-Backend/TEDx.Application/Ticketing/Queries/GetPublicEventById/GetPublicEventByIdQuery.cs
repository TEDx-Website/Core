using MediatR;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Queries.GetPublicEventById
{
    public sealed record GetPublicEventByIdQuery(Guid Id)
        : IRequest<Result<GetPublicEventByIdResponse>>;
}

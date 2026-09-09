using MediatR;
using TEDx.Application.Common.Interfaces.Authorization;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Queries.GetContactSubmissionById
{
    public sealed record GetContactSubmissionByIdQuery(Guid Id)
    : IRequireAdmin, IRequest<Result<GetContactSubmissionByIdResponse>>;
}

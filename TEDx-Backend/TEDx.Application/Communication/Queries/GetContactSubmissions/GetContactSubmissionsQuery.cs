using MediatR;
using TEDx.Application.Common.Interfaces.Authorization;
using TEDx.Application.Common.Pagination;
using TEDx.Application.Communication.Dtos;
using TEDx.Domain.Common;

namespace TEDx.Application.Communication.Queries.GetContactSubmissions;

public sealed record GetContactSubmissionsQuery(
    int? Page = null,
    int? PageSize = null,
    string? Status = null,
    string? Sort = null)
    : IRequest<Result<PagedResult<ContactSubmissionListItemDto>>>, IRequireAdmin;

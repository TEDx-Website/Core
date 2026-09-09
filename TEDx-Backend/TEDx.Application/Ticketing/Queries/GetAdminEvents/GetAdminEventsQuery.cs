using MediatR;
using TEDx.Application.Common.Interfaces.Authorization;
using TEDx.Application.Common.Pagination;
using TEDx.Application.Ticketing.Dtos;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Queries.GetAdminEvents;

public sealed record GetAdminEventsQuery(
    int? Page = null,
    int? PageSize = null,
    string? Sort = null,
    string? Status = null,
    string? Search = null)
    : IRequest<Result<PagedResult<AdminEventListItemDto>>>, IRequireAdmin;

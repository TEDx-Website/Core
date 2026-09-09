using MediatR;
using TEDx.Application.Common.Interfaces.Authorization;
using TEDx.Application.Common.Pagination;
using TEDx.Application.Ticketing.Dtos;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Queries.GetEventPromoCodes;

public sealed record GetEventPromoCodesQuery(
    Guid EventId,
    int? Page = null,
    int? PageSize = null
) : IRequest<Result<PagedResult<EventPromoCodeDto>>>, IRequireAdmin;

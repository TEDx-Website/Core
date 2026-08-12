using MediatR;
using TEDx.Application.Common.Interfaces.Authorization;
using TEDx.Application.Common.Pagination;
using TEDx.Application.Ticketing.DTOs;
using TEDx.Domain.Common;
using TEDx.Domain.Ticketing.Enums;
namespace TEDx.Application.Ticketing.Queries.GetEventOrders
{
    public sealed record GetEventOrdersQuery(
    Guid EventId,
    int Page = 1,
    int PageSize = 20,
    OrderStatus? Status = null
) : IRequest<Result<PagedResult<EventOrderDto>>>, IRequireAdmin;
}

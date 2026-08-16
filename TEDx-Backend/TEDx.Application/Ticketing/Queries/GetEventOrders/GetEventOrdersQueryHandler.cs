using MediatR;
using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.Interfaces;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Pagination;
using TEDx.Application.Ticketing.DTOs;
using TEDx.Domain.Common;
using TEDx.Domain.Ticketing.Enums;
using TEDx.Application.Common.DTOs;

namespace TEDx.Application.Ticketing.Queries.GetEventOrders;

public sealed class GetEventOrdersQueryHandler(
    IAppDbContext context)
    : IRequestHandler<
        GetEventOrdersQuery,
        Result<PagedResult<EventOrderDto>>>
{
    public async Task<Result<PagedResult<EventOrderDto>>> Handle(
        GetEventOrdersQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Check event exists and is not soft deleted
        var eventExists = await context.Events
            .AsNoTracking()
            .AnyAsync(
                e => e.Id == request.EventId,
                cancellationToken);


        if (!eventExists)
        {
            return Result<PagedResult<EventOrderDto>>.Failure(
                Errors_Common.NotFound);
        }

        // 2. Build orders query
        var query = context.Orders
            .AsNoTracking()
            .Where(o => o.EventId == request.EventId);

        // 3. Optional status filter
        if (request.Status.HasValue)
        {
            query = query.Where(o => o.Status == request.Status.Value);
        }

        // 4. Total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        var page = PageRequest.From(request.Page, request.PageSize);

        var orders = await query
           .OrderByDescending(o => o.CreatedAtUtc)
           .Skip(page.Skip)
           .Take(page.Take)
           .Select(o => new EventOrderDto(
               o.Id,
               o.AccountId ,
               o.Status.ToString(),
               o.Quantity,
               new MoneyDto(o.TotalSnapshot,"EGP"),
               o.CreatedAtUtc,
               o.Status == OrderStatus.PendingPayment ? o.HoldExpiresAtUtc : null))
           .ToListAsync(cancellationToken);

        return Result<PagedResult<EventOrderDto>>.Success(
            PagedResult<EventOrderDto>.Create(orders, page, totalCount));
    }
}

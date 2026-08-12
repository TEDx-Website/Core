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

//public sealed class GetEventOrdersQueryHandler(
//    IAppDbContext context)
//    : IRequestHandler<
//        GetEventOrdersQuery,
//        Result<PagedResult<EventOrderDto>>>
//{
    //public async Task<Result<PagedResult<EventOrderDto>>> Handle(
    //    GetEventOrdersQuery request,
    //    CancellationToken cancellationToken)
    //{
    //    // 1. Check event exists and is not soft deleted
    //    var eventExists = await context.Events
    //        .AsNoTracking()
    //        .AnyAsync(
    //            e => e.Id == request.EventId && !e.IsDeleted,
    //            cancellationToken);

    //    if (!eventExists)
    //    {
    //        return Result<PagedResult<EventOrderDto>>.Failure(
    //            Errors_Common.NotFound);
    //    }

    //    // 2. Build orders query
    //    var query = context.Orders
    //        .AsNoTracking()
    //        .Where(o => o.EventId == request.EventId);

    //    // 3. Optional status filter
    //    if (request.Status.HasValue)
    //    {
    //        query = query.Where(o => o.Status == request.Status.Value);
    //    }

    //    // 4. Total count before pagination
    //    var totalCount = await query.CountAsync(cancellationToken);

        //return Result<PagedResult<EventOrderDto>>.Success(
        //    PagedResult<EventOrderDto>.Create(items, page, totalCount));
//    }
//}

/*
    // 5. Pagination
        var items = await query
            .OrderByDescending(o => o.CreatedAtUtc)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(o => new EventOrderDto
            {
                Buyer = new BuyerDTO(o.AccountId, ),
                Status = o.Status.ToString(),
                Quantity = o.Quantity,
                Total = new MoneyDto(o.TotalSnapshot, o.Currency),
                CreatedAtUtc = o.CreatedAtUtc,
                HoldExpiresAtUtc = o.Status == OrderStatus.PendingPayment ? o.HoldExpiresAtUtc : null
            })
            .ToListAsync(cancellationToken);
        var page = new PageRequest(request.Page, request.PageSize);
 */

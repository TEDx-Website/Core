using MediatR;
using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Application.Common.Pagination;
using TEDx.Application.Ticketing.Dtos;
using TEDx.Domain.Common;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Application.Ticketing.Queries.GetEventPromoCodes;

public sealed class GetEventPromoCodesQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetEventPromoCodesQuery, Result<PagedResult<EventPromoCodeDto>>>
{
    public async Task<Result<PagedResult<EventPromoCodeDto>>> Handle(
        GetEventPromoCodesQuery request,
        CancellationToken cancellationToken)
    {
        var eventExists = await db.Events
            .AsNoTracking()
            .AnyAsync(e => e.Id == request.EventId, cancellationToken);

        if (!eventExists)
            return Result<PagedResult<EventPromoCodeDto>>.Failure(CommonErrors.NotFound);

        var page = await db.PromoCodes
            .AsNoTracking()
            .Where(p => p.EventId == request.EventId)
            .OrderBy(p => p.Code)
            .Select(p => new EventPromoCodeDto(
                p.Id,
                p.Code,
                p.DiscountType,
                p.DiscountValue,
                p.PromoRedemptions!.Count(r =>
                    r.Status == PromoRedemptionStatus.Claimed
                    || r.Status == PromoRedemptionStatus.Confirmed),
                p.MaxTotalRedemption,
                p.MaxPerUser,
                p.IsActive,
                p.ValidFromUtc,
                p.ValidUntilUtc))
            .ToPagedResultAsync(
                PagedRequest.From(request.Page, request.PageSize),
                cancellationToken);

        return Result<PagedResult<EventPromoCodeDto>>.Success(page);
    }
}

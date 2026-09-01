using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Application.Common.Pagination;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Queries.GetPublicEvents
{
    public sealed record GetPublicEventsQuery(
        int? Page,
        int? PageSize,
        string? Sort,
        string? When
        ) : IRequest<Result<PagedResult<GetPublicEventResponse>>>;

}

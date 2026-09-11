using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Application.Common.Pagination;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Queries.GetAdminSpeakers
{
    public sealed record GetAdminSpeakersQuery(
    int? Page,
    int? PageSize)
    : IRequest<Result<PagedResult<AdminSpeakerResponse>>>;
}

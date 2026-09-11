using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Queries.GetTopSpeakers
{
    public sealed record GetTopSpeakersQuery(int? Limit = null)
        : IRequest<Result<List<TopSpeakerResponse>>>;
}

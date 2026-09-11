using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Application.Ticketing.Queries.GetTopSpeakers
{
    public sealed record TopSpeakerResponse(
     Guid Id,
     string? Name,
     string? PictureUrl,
     string? TagLine,
     string? Description,
     List<string?> EventTitles);
}

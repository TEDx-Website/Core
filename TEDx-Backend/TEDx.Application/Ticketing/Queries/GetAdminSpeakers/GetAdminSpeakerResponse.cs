using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Application.Ticketing.Queries.GetAdminSpeakers
{
    public sealed record AdminSpeakerResponse(
    Guid Id,
    string? Name,
    string? PictureUrl,
    string? TagLine,
    string? Description,
    List<string?> EventTitles   
);
}

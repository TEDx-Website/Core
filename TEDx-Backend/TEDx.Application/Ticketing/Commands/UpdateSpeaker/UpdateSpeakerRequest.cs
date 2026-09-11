using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Application.Ticketing.Commands.UpdateSpeaker
{
    public sealed record UpdateSpeakerRequest(
    string Name,
    string PictureUrl,
    string TagLine,
    string Description,
    List<Guid> EventIds);
}

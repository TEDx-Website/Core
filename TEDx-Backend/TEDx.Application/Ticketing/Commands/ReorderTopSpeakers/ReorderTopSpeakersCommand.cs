using System.Collections.Generic;
using MediatR;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Commands.ReorderTopSpeakers
{
    public sealed record ReorderTopSpeakersCommand(IReadOnlyList<TopSpeakerOrderItem> Items)
        : IRequest<Result<Unit>>;
}

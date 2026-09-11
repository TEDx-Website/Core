using Microsoft.EntityFrameworkCore;
using MediatR;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Queries.GetTopSpeakers
{
    public sealed class GetTopSpeakersQueryHandler(
    IApplicationDbContext context)
    : IRequestHandler<
        GetTopSpeakersQuery,
        Result<List<TopSpeakerResponse>>>
    {
        public async Task<Result<List<TopSpeakerResponse>>> Handle(
            GetTopSpeakersQuery request,
            CancellationToken cancellationToken)
        {
            var query = context.Speakers
                .AsNoTracking()
                .Where(s => s.IsTopSpeaker)
                .OrderBy(s => s.TopSpeakerOrderIndex)
                .AsQueryable();

            if (request.Limit.HasValue && request.Limit.Value > 0)
            {
                query = query.Take(request.Limit.Value);
            }

            var speakers = await query
                .Select(s => new TopSpeakerResponse(
                    s.Id,
                    s.SpeakerName,
                    s.SpeakerPictureUrl,
                    s.SpeakerRole,
                    s.TalkTitle,
                    s.TalkTrack,
                    s.TalkShortDescription,
                    s.TopSpeakerOrderIndex ?? 0
                ))
                .ToListAsync(cancellationToken);

            return Result<List<TopSpeakerResponse>>.Success(speakers);
        }
    }
}

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
            var speakers = await context.Speakers
                .AsNoTracking()
                .Where(s => s.IsTopSpeaker)
                .OrderBy(s => s.Name)
                .Select(s => new TopSpeakerResponse(
                    s.Id,
                    s.Name,
                    s.PictureUrl,
                    s.TagLine,
                    s.Description,
                    s.Events
                        .Select(e => e.TitleEn)
                        .ToList()
                ))
                .ToListAsync(cancellationToken);

            return Result<List<TopSpeakerResponse>>.Success(speakers);
        }
    }
}

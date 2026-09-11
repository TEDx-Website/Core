using MediatR;
using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Queries.GetSpeakers
{
    public sealed class GetSpeakersQueryHandler(
        IApplicationDbContext context)
        : IRequestHandler<GetSpeakersQuery, Result<SpeakerCatalogResponse>>
    {
        public async Task<Result<SpeakerCatalogResponse>> Handle(
            GetSpeakersQuery request,
            CancellationToken cancellationToken)
        {
            var query = context.Speakers.AsNoTracking();

            // Apply search filter (name or talk title)
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var term = request.Search.Trim().ToLower();
                query = query.Where(s =>
                    s.SpeakerName.ToLower().Contains(term) ||
                    s.TalkTitle.ToLower().Contains(term));
            }

            // Apply category / track filter
            if (!string.IsNullOrWhiteSpace(request.Category) &&
                !request.Category.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                var cat = request.Category.Trim().ToLower();
                query = query.Where(s => s.TalkTrack.ToLower().Contains(cat));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var speakers = await query
                .OrderBy(s => s.SpeakerName)
                .Select(s => new SpeakerCatalogItem(
                    s.Id,
                    s.SpeakerName,
                    s.SpeakerPictureUrl,
                    s.SpeakerRole,
                    s.TalkTrack,
                    s.TalkTitle,
                    s.TalkShortDescription,
                    s.SpeakerBio,
                    new SocialLinksResponse(
                        s.SpeakerLinkedInUrl,
                        s.SpeakerXUrl,
                        s.SpeakerWebsiteUrl)))
                .ToListAsync(cancellationToken);

            return Result<SpeakerCatalogResponse>.Success(
                new SpeakerCatalogResponse(totalCount, speakers));
        }
    }
}

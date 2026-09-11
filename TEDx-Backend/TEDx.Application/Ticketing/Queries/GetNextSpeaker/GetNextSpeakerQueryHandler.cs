using MediatR;
using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Queries.GetNextSpeaker
{
    public sealed class GetNextSpeakerQueryHandler(
        IApplicationDbContext context)
        : IRequestHandler<GetNextSpeakerQuery, Result<NextSpeakerResponse>>
    {
        public async Task<Result<NextSpeakerResponse>> Handle(
            GetNextSpeakerQuery request,
            CancellationToken cancellationToken)
        {
            var speaker = await context.Speakers
                .AsNoTracking()
                .Where(s => s.IsNextSpeaker)
                .Select(s => new NextSpeakerResponse(
                    s.Id,
                    s.TalkTrack,
                    s.SpeakerName,
                    s.SpeakerRole,
                    s.TalkTitle,
                    s.SpeakerBio,
                    s.SpeakerPictureUrl))
                .FirstOrDefaultAsync(cancellationToken);

            if (speaker is null)
                return Result<NextSpeakerResponse>.Failure(CommonErrors.NotFound);

            return Result<NextSpeakerResponse>.Success(speaker);
        }
    }
}

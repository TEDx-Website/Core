using MediatR;
using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Commands.UpdateSpeaker
{
    public sealed class UpdateSpeakerCommandHandler(
        IApplicationDbContext context)
        : IRequestHandler<UpdateSpeakerCommand, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(
            UpdateSpeakerCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Get the speaker
            var speaker = await context.Speakers
                .FirstOrDefaultAsync(
                    s => s.Id == request.SpeakerId,
                    cancellationToken);

            if (speaker is null)
            {
                return Result<Unit>.Failure(CommonErrors.NotFound);
            }

            // 2. Update speaker data
            speaker.SpeakerName = request.SpeakerName;
            speaker.SpeakerPictureUrl = request.SpeakerPictureUrl;
            speaker.SpeakerRole = request.SpeakerRole;
            speaker.SpeakerBio = request.SpeakerBio;
            speaker.TalkTitle = request.TalkTitle;
            speaker.TalkTrack = request.TalkTrack;
            speaker.TalkShortDescription = request.TalkShortDescription;
            speaker.SpeakerLinkedInUrl = request.SpeakerLinkedInUrl;
            speaker.SpeakerXUrl = request.SpeakerXUrl;
            speaker.SpeakerWebsiteUrl = request.SpeakerWebsiteUrl;

            // 3. Save changes
            await context.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}

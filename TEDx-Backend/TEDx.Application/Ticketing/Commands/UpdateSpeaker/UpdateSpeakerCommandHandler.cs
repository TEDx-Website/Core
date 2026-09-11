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
            speaker.Name = request.Name;
            speaker.PictureUrl = request.PictureUrl;
            speaker.TagLine = request.TagLine;
            speaker.Description = request.Description;

            // 3. Save changes
            await context.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}

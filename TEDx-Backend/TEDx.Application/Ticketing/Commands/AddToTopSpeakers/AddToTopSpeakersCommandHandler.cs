using MediatR;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace TEDx.Application.Ticketing.Commands.AddToTopSpeakers
{
    public sealed class AddToTopSpeakerCommandHandler(
    IApplicationDbContext context)
    : IRequestHandler<AddTopSpeakerCommand, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(
            AddTopSpeakerCommand request,
            CancellationToken cancellationToken)
        {
            var speaker = await context.Speakers
                .FirstOrDefaultAsync(
                    s => s.Id == request.SpeakerId,
                    cancellationToken);

            if (speaker is null)
            {
                return Result<Unit>.Failure(CommonErrors.NotFound);
            }

            if (speaker.IsTopSpeaker)
            {
                return Result<Unit>.Failure(TicketingErrors.IsAlreadyTop);
            }

            var topSpeakersCount = await context.Speakers
                .CountAsync(
                    s => s.IsTopSpeaker,
                    cancellationToken);

            if (topSpeakersCount >= 5)
            {
                return Result<Unit>.Failure(CommonErrors.ValidationError);
            }

            speaker.IsTopSpeaker = true;

            await context.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}

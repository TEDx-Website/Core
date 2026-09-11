using MediatR;
using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Commands.SetNextSpeaker
{
    public sealed class SetNextSpeakerCommandHandler(
        IApplicationDbContext context)
        : IRequestHandler<SetNextSpeakerCommand, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(
            SetNextSpeakerCommand request,
            CancellationToken cancellationToken)
        {
            var speaker = await context.Speakers
                .FirstOrDefaultAsync(s => s.Id == request.SpeakerId, cancellationToken);

            if (speaker is null)
                return Result<Unit>.Failure(CommonErrors.NotFound);

            // Unset any previously designated next speaker
            var previous = await context.Speakers
                .Where(s => s.IsNextSpeaker && s.Id != request.SpeakerId)
                .ToListAsync(cancellationToken);

            foreach (var prev in previous)
                prev.IsNextSpeaker = false;

            speaker.IsNextSpeaker = true;

            await context.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}

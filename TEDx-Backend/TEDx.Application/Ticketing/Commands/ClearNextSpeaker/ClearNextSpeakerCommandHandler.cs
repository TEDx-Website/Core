using MediatR;
using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Commands.ClearNextSpeaker
{
    public sealed class ClearNextSpeakerCommandHandler(
        IApplicationDbContext context)
        : IRequestHandler<ClearNextSpeakerCommand, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(
            ClearNextSpeakerCommand request,
            CancellationToken cancellationToken)
        {
            var speakers = await context.Speakers
                .Where(s => s.IsNextSpeaker)
                .ToListAsync(cancellationToken);

            foreach (var speaker in speakers)
                speaker.IsNextSpeaker = false;

            await context.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}

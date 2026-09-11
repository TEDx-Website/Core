using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace TEDx.Application.Ticketing.Commands.RemoveTopSpeaker
{
    public sealed class RemoveTopSpeakerCommandHandler(
     IApplicationDbContext context)
     : IRequestHandler<RemoveTopSpeakerCommand, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(
            RemoveTopSpeakerCommand request,
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

            if (!speaker.IsTopSpeaker)
            {
                return Result<Unit>.Failure(TicketingErrors.NotTopSpeaker);
            }

            speaker.IsTopSpeaker = false;
            speaker.TopSpeakerOrderIndex = null;

            var remainingTopSpeakers = await context.Speakers
                .Where(s => s.IsTopSpeaker && s.Id != speaker.Id)
                .OrderBy(s => s.TopSpeakerOrderIndex)
                .ToListAsync(cancellationToken);

            for (int i = 0; i < remainingTopSpeakers.Count; i++)
            {
                remainingTopSpeakers[i].TopSpeakerOrderIndex = i + 1;
            }

            await context.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}

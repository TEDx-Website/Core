using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Commands.ReorderTopSpeakers
{
    public sealed class ReorderTopSpeakersCommandHandler(
        IApplicationDbContext context)
        : IRequestHandler<ReorderTopSpeakersCommand, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(
            ReorderTopSpeakersCommand request,
            CancellationToken cancellationToken)
        {
            if (request.Items == null || request.Items.Count == 0)
            {
                return Result<Unit>.Failure(CommonErrors.ValidationError);
            }

            // Check for negative/zero order index or duplicate speaker IDs or duplicate order indices
            if (request.Items.Any(i => i.OrderIndex <= 0) ||
                request.Items.Select(i => i.SpeakerId).Distinct().Count() != request.Items.Count ||
                request.Items.Select(i => i.OrderIndex).Distinct().Count() != request.Items.Count)
            {
                return Result<Unit>.Failure(CommonErrors.ValidationError);
            }

            var speakerIds = request.Items.Select(i => i.SpeakerId).ToList();

            var speakers = await context.Speakers
                .Where(s => speakerIds.Contains(s.Id))
                .ToListAsync(cancellationToken);

            if (speakers.Count != speakerIds.Count)
            {
                return Result<Unit>.Failure(CommonErrors.NotFound);
            }

            if (speakers.Any(s => !s.IsTopSpeaker))
            {
                return Result<Unit>.Failure(TicketingErrors.SpeakerNotInTopList);
            }

            var itemDict = request.Items.ToDictionary(i => i.SpeakerId, i => i.OrderIndex);

            foreach (var speaker in speakers)
            {
                speaker.TopSpeakerOrderIndex = itemDict[speaker.Id];
            }

            await context.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}

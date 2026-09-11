using MediatR;
using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;
namespace TEDx.Application.Ticketing.Commands.DeleteSpeaker;

public sealed class DeleteSpeakerCommandHandler(
    IApplicationDbContext context)
    : IRequestHandler<DeleteSpeakerCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(
        DeleteSpeakerCommand request,
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

        var hasEvents = await context.Events
            .AnyAsync(
                e => e.SpeakerId == request.SpeakerId,
                cancellationToken);

        if (hasEvents)
        {
            return Result<Unit>.Failure(TicketingErrors.SpeakerHasEvents);
        }

        context.Speakers.Remove(speaker);

        await context.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;
using TEDx.Domain.Ticketing.Entities;

namespace TEDx.Application.Ticketing.Commands.CreateSpeaker
{

    public sealed class CreateSpeakerCommandHandler(
        IApplicationDbContext context)
        : IRequestHandler<CreateSpeakerCommand, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(
            CreateSpeakerCommand request,
            CancellationToken cancellationToken)
        {
            var speaker = new Speaker
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                PictureUrl = request.PictureUrl,
                TagLine = request.TagLine,
                Description = request.Description
            };

            context.Speakers.Add(speaker);

            await context.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Domain.Common;

namespace TEDx.Application.Common.Interfaces.Authorization
{
    public sealed record GradeSubmissionCommand(
    Guid TrackId,
    Guid SubmissionId,
    int Score) : IRequest<Error>, IRequireBoardOfTrack;
}

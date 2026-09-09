using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Training.Enums;

namespace TEDx.Application.Common.Interfaces
{
    public interface ITrackAccessReader
    {
        Task<TrackRole?> GetRoleInTrackAsync(Guid accountId, Guid trackId, CancellationToken ct);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Training.Enums;

namespace TEDx.Infrastructure.Identity
{
    public sealed class TrackAccessReader : ITrackAccessReader
    {
        private readonly IAppDbContext _db;
        private readonly IClock _clock;
        public TrackAccessReader(IAppDbContext db, IClock clock)
        {
            _db = db;
            _clock = clock;
        }

        public async Task<TrackRole?> GetRoleInTrackAsync(Guid accountId, Guid trackId, CancellationToken ct)
        {
            var now = _clock.UtcNow;

            var assignment = await _db.TrackAssignments
                .AsNoTracking()
                .Where(a => a.AccountId == accountId
                         && a.TrackId == trackId
                         && a.StartAtUtc <= now
                         && (a.EndAtUtc == null || a.EndAtUtc > now))
                .Select(a => new { a.TrackRole })
                .FirstOrDefaultAsync(ct);

            return assignment?.TrackRole;
        }
    }
}

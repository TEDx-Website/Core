using TEDx.Application.Identity.Queries.GetMyProfile;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Application.Identity.Dtos;
using TEDx.Domain.Common;
using TEDx.Domain.Training.Enums;

namespace TEDx.Application.Identity.Services
{
    public sealed class MyProfileService : IMyProfileService
    {
        private readonly IApplicationDbContext _appDbContext;
        private readonly IClock _clock;

        public MyProfileService(
            IApplicationDbContext appDbContext,
            IClock clock)
        {
            _appDbContext = appDbContext;
            _clock = clock;
        }

        public async Task<MyProfileResponse?> GetMyProfileAsync(
            Guid userId,
            CancellationToken ct)
        {
            var user = await _appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId, ct);
            if (user == null)
            {
                return null;
            }
            var now = _clock.UtcNow;

            // Get active track assignments for the user
            var activeAssignments = await _appDbContext.TrackAssignments
                .AsNoTracking()
                .Where(a =>
                    a.AccountId == userId &&
                    a.StartAtUtc <= now &&
                    (a.EndAtUtc == null || a.EndAtUtc >= now))
                .ToListAsync(ct);

            var assignments = new TrackAssignmentDto(
                MemberOfTrackId: activeAssignments.FirstOrDefault(a => a.TrackRole == TrackRole.Member)?.TrackId,
                BoardOfTrackId: activeAssignments.FirstOrDefault(a => a.TrackRole == TrackRole.Board)?.TrackId);

            var response = new MyProfileResponse(
                Id: user.Id,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Email: user.Email,
                Phone: user.PhoneNumber,
                Bio: user.Bio,
                ProfilePictureUrl: user.ProfilePictureUrl,
                GlobalRole: user.Role,
                Assignments: assignments);
            return response;
        }
    }
}

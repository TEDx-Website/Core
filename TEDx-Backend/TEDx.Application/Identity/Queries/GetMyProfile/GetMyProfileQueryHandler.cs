using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Application.Identity.Commands.ChangePassword;
using TEDx.Application.Identity.DTOs;
using TEDx.Domain.Common;
using TEDx.Domain.Training.Enums;

namespace TEDx.Application.Identity.Queries.GetMyProfile
{
    public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, Result<MyProfileDTO>>
    {
        private readonly IAppDbContext appDbContext;
        private readonly ICurrentUser currentUser;
        private readonly IClock clock;
        private readonly ILogger<GetMyProfileQueryHandler> logger;

        public GetMyProfileQueryHandler(IAppDbContext _appDbContext, ICurrentUser user, IClock _clock, ILogger<GetMyProfileQueryHandler> _logger)
        {
            appDbContext = _appDbContext;
            currentUser = user;
            clock = _clock;
            logger = _logger;
        }
        public async Task<Result<MyProfileDTO>> Handle(GetMyProfileQuery request, CancellationToken ct)
        {
            var UserId = currentUser.UserId;
            var User = await appDbContext.ApplicationUsers.Where(x => x.Id == UserId).FirstOrDefaultAsync();
            if(User == null)
            {
                return Result<MyProfileDTO>.Failure(Errors_Identity.UserNotFound);
            }
            var assignments = await appDbContext.TrackAssignments.Where(a => a.AccountId == UserId
                                                                        && a.StartAtUtc <= clock.UtcNow && a.EndAtUtc >= clock.UtcNow)
                                                                .Select(a => new TrackAssignmentDTO
                                                                {
                                                                    BoardOfTrackId = a.TrackRole == TrackRole.Board ? a.TrackId : Guid.Empty,
                                                                    MemberOfTrackId = a.TrackRole == TrackRole.Member ? a.TrackId : Guid.Empty
                                                                }).ToListAsync(ct);

            var response = new MyProfileDTO
            {
                Id = User.Id,
                FirstName = User.FirstName,
                LastName = User.LastName,
                Email = User.Email,
                Phone = User.PhoneNumber,
                Bio = User.Bio,
                ProfilePictureUrl = User.ProfilePictureUrl,
                Role = User.Role,
                Assignments = assignments
            };

            return Result<MyProfileDTO>.Success(response);
        }
    }
}

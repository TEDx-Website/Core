using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Application.Identity.Commands.ChangePassword;
using TEDx.Application.Identity.Dtos;
using TEDx.Application.Identity.Services;
using TEDx.Domain.Common;
using TEDx.Domain.Training.Enums;

namespace TEDx.Application.Identity.Queries.GetMyProfile
{
    public sealed class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, Result<MyProfileResponse>>
    {
        private readonly IApplicationDbContext appDbContext;
        private readonly ICurrentUser currentUser;
        private readonly IClock clock;
        private readonly ILogger<GetMyProfileQueryHandler> logger;
        private readonly IMyProfileService myProfileService;

        public GetMyProfileQueryHandler(IApplicationDbContext _appDbContext, ICurrentUser user, IClock _clock, ILogger<GetMyProfileQueryHandler> _logger, IMyProfileService _myProfileService)
        {
            appDbContext = _appDbContext;
            currentUser = user;
            clock = _clock;
            logger = _logger;
            myProfileService = _myProfileService;
        }
        public async Task<Result<MyProfileResponse>> Handle(GetMyProfileQuery request, CancellationToken ct)
        {
            var userId = currentUser.UserId;
            if (userId == null || userId == Guid.Empty)
            {
                return Result<MyProfileResponse>.Failure(IdentityErrors.UserNotFound);
            }

            var profile = await myProfileService.GetMyProfileAsync(userId.Value, ct);

            if (profile is null)
            {
                return Result<MyProfileResponse>.Failure(IdentityErrors.UserNotFound);
            }
            return Result<MyProfileResponse>.Success(profile);
        }
    }
}

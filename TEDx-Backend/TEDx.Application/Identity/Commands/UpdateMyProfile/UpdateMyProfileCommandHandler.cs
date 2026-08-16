using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Application.Identity.DTOs;
using TEDx.Domain.Common;
using TEDx.Domain.Identity.Entities;
using TEDx.Domain.Training.Enums;
using TEDx.Application.Identity.Service;


namespace TEDx.Application.Identity.Commands.UpdateProfile
{
    public class UpdateMyProfileCommandHandler : IRequestHandler<UpdateMyProfileCommand, Result<MyProfileDTO>>
    {
        private readonly IAppDbContext _context;
        private readonly ICurrentUser _user;
        private readonly IMyProfileService _myProfileService;

        public UpdateMyProfileCommandHandler(IAppDbContext context, ICurrentUser user, IMyProfileService myProfileService)
        {
            _context = context;
            _user = user;
            _myProfileService = myProfileService;
        }
        public async Task<Result<MyProfileDTO>> Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
        {
            var UserId = _user.UserId;

            var User = await _context.ApplicationUsers
                .FirstOrDefaultAsync(x => x.Id == UserId, cancellationToken);
            if (User == null)
            {
                return Result<MyProfileDTO>.Failure(Errors_Identity.UserNotFound);
            }

            User.FirstName = request.FirstName;
            User.LastName = request.LastName;
            User.PhoneNumber = request.Phone;
            User.Bio = request.Bio;

            await _context.SaveChangesAsync(cancellationToken);
            var profile = await _myProfileService.GetMyProfileAsync(UserId.Value, cancellationToken);

            if (profile is null)
            {
                return Result<MyProfileDTO>.Failure(
                    Errors_Identity.UserNotFound);
            }

            return Result<MyProfileDTO>.Success(profile);
        }
    }
    
}

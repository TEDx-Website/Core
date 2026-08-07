using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;
using TEDx.Domain.Identity.Entities;
using MediatR;
using TEDx.Application.Identity.DTOs;
using TEDx.Application.Common.Errors;

namespace TEDx.Application.Identity.Commands.UpdateProfile
{
    public class UpdateMyProfileCommandHandler : IRequestHandler<UpdateMyProfileCommand, Result<MyProfileDTO>>
    {
        private readonly IAppDbContext _context;
        private readonly ICurrentUser _user;

        public UpdateMyProfileCommandHandler(IAppDbContext context, ICurrentUser user)
        {
            _context = context;
            _user = user;
        }
        public async Task<Result<MyProfileDTO>> Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
        {
            var UserId = _user.UserId;
            var User = await _context.ApplicationUsers.Where(u => u.Id == UserId).FirstOrDefaultAsync();

            if (User == null)
            {
                return Result<MyProfileDTO>.Failure(Errors_Identity.UserNotFound);
            }

            User.FirstName = request.FirstName;
            User.LastName = request.LastName;
            User.PhoneNumber = request.Phone;
            User.Bio = request.Bio;

            await _context.SaveChangesAsync(cancellationToken);

            var profileDto = new MyProfileDTO
            {
                Id = User.Id,
                FirstName = User.FirstName,
                LastName = User.LastName,
                Email = User.Email ,
                Bio = User.Bio,
                Phone = User.PhoneNumber,
            };

            return Result<MyProfileDTO>.Success(profileDto);
        }
    }
}

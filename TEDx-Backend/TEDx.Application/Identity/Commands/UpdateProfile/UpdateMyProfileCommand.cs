using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Application.Identity.DTOs;
using TEDx.Domain.Common;

namespace TEDx.Application.Identity.Commands.UpdateProfile
{
    public class UpdateMyProfileCommand : IRequest<Result<MyProfileDTO>>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Bio { get; set; }
    }
}

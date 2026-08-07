using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Application.Identity.DTOs;
using TEDx.Domain.Common;
namespace TEDx.Application.Identity.Queries.GetMyProfile
{
    public class GetMyProfileQuery : IRequest<Result<MyProfileDTO>>
    {

    }

}

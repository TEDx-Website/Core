using TEDx.Application.Identity.Queries.GetMyProfile;
using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Application.Identity.Dtos;

namespace TEDx.Application.Identity.Services
{
    public interface IMyProfileService
    {
        Task<MyProfileResponse?> GetMyProfileAsync(
            Guid userId,
            CancellationToken ct);
    }
}

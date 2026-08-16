using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Application.Identity.DTOs;

namespace TEDx.Application.Identity.Service
{
    public interface IMyProfileService
    {
        Task<MyProfileDTO?> GetMyProfileAsync(
            Guid userId,
            CancellationToken ct);
    }
}

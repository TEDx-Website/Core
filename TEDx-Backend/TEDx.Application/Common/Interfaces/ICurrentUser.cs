using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Identity.Enums;

namespace TEDx.Application.Common.Interfaces
{
    public interface ICurrentUser
    {
        Guid? UserId { get; }
        string? Email { get; }
        GlobalRole? Role { get; }
        bool IsAuthenticated { get; }
        bool IsAdmin { get; }
    }
}

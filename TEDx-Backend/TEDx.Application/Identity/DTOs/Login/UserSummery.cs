using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Application.Identity.DTOs.Login
{

    public sealed record UserSummary(
        Guid Id, string Email, string GlobalRole, string FirstName, string LastName);
}

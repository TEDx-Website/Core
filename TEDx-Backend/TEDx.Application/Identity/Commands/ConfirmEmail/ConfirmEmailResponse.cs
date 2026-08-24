using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Application.Identity.Commands.ConfirmEmail
{
    public sealed record ConfirmEmailResponse(string Email, bool EmailConfirmed);
}

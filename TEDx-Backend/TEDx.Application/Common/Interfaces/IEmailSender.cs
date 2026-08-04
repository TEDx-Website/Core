using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Application.Common.Interfaces
{
    public interface IEmailSender
    {
        Task SendPasswordResetEmailAsync(
           string to,
           string resetLink,
           CancellationToken cancellationToken = default);

        Task SendEmailConfirmationEmailAsync(
           string to,
           string confirmLink,
           CancellationToken cancellationToken = default);
    }
}

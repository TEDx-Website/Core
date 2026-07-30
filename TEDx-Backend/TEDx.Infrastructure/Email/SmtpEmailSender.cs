using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using TEDx.Infrastructure.Configuration;

namespace TEDx.Infrastructure.Email
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpOptions _option;
        public SmtpEmailSender(IOptions<SmtpOptions> options)
        {
            _option = options.Value;
        }
        public Task SendPasswordResetEmailAsync(
           string to,
           string resetLink,
           CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

    }
}

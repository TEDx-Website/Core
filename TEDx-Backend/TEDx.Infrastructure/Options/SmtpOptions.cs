using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.Extensions.Options;
using TEDx.Application.Common;

namespace TEDx.Infrastructure.Options
{
    public sealed class SmtpOptions
    {
        public const string SectionName = "Smtp";
        [Required]
        public string Host { get; init; } = default!;
        [Range(1, 65535)]
        public int Port { get; init; }
        [Required]
        public string Username { get; init; } = default!;
        [Required]
        public string Password { get; init; } = default!;
        [Required]
        [EmailAddress]
        public string FromAddress { get; init; } = default!;
        public string FromName { get; init; } = string.Empty;
    }

    public sealed class SmtpOptionsValidator : IValidateOptions<SmtpOptions>
    {
        public ValidateOptionsResult Validate(string? name, SmtpOptions options)
        {
            var failures = new List<string>();

            if (string.IsNullOrWhiteSpace(options.Host))
                failures.Add($"{SmtpOptions.SectionName}:{nameof(SmtpOptions.Host)} is required.");

            if (string.IsNullOrWhiteSpace(options.Username))
                failures.Add($"{SmtpOptions.SectionName}:{nameof(SmtpOptions.Username)} is required.");

            if (string.IsNullOrWhiteSpace(options.Password))
                failures.Add($"{SmtpOptions.SectionName}:{nameof(SmtpOptions.Password)} is required.");

            if (string.IsNullOrWhiteSpace(options.FromAddress))
                failures.Add($"{SmtpOptions.SectionName}:{nameof(SmtpOptions.FromAddress)} is required.");

            if (options.Port <= 0 || options.Port > 65535)
                failures.Add("Smtp:Port must be between 1 and 65535.");

            return failures.Count > 0
                ? ValidateOptionsResult.Fail(failures)
                : ValidateOptionsResult.Success;
        }
    }
}

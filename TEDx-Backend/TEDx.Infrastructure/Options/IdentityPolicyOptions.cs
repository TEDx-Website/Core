using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;

namespace TEDx.Infrastructure.Options
{
    public sealed class IdentityPolicyOptions
    {
        public const string SectionName = "IdentityPolicy";

        public int PasswordMinLength { get; init; } = 8;
        public int MaxFailedAttempts { get; init; } = 5;
        public int LockoutMinutes { get; init; } = 15;
        public int ResetTokenHours { get; init; } = 1;    // NFR-SEC-02
        public int ConfirmTokenHours { get; init; } = 24;   // D:Q57
    }

    public sealed class IdentityPolicyOptionsValidator : IValidateOptions<IdentityPolicyOptions>
    {
        public ValidateOptionsResult Validate(string? name, IdentityPolicyOptions options)
        {
            var failures = new List<string>();

            if (options.PasswordMinLength < 8)
                failures.Add($"{IdentityPolicyOptions.SectionName}:PasswordMinLength must be >= 8 (FR-AUTH-03).");

            if (options.MaxFailedAttempts < 1)
                failures.Add($"{IdentityPolicyOptions.SectionName}:MaxFailedAttempts must be >= 1.");

            if (options.LockoutMinutes < 1)
                failures.Add($"{IdentityPolicyOptions.SectionName}:LockoutMinutes must be >= 1.");


            if (options.ResetTokenHours is < 1 or > 4)
                failures.Add($"{IdentityPolicyOptions.SectionName}:ResetTokenHours must be 1..4 (NFR-SEC-02).");


            if (options.ConfirmTokenHours is < 1 or > 72)
                failures.Add($"{IdentityPolicyOptions.SectionName}:ConfirmTokenHours must be 1..72.");


            return failures.Count > 0
                ? ValidateOptionsResult.Fail(failures)
                : ValidateOptionsResult.Success;
        }
    }
}

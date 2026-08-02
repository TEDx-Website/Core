using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.Extensions.Options;

namespace TEDx.Infrastructure.Configuration
{
    public sealed class JwtOptions
    {
        public const string SectionName = "Jwt";

        // HMAC-SHA256 is a 256-bit algorithm, so a shorter key is a real weakness —
        public const int MinKeyBytes = 32;

        [Required]
        public string Issuer { get; init; } = default!;

        [Required]
        public string Audience { get; init; } = default!;

        [Required]
        public string Key { get; init; } = default!;

        [Range(1, 1440)]
        public int AccessTokenMinutes { get; init; } = 15;
    }

    public sealed class JwtOptionsValidator : IValidateOptions<JwtOptions>
    {
        public ValidateOptionsResult Validate(string? name, JwtOptions options)
        {
            var failures = new List<string>();

            if (string.IsNullOrWhiteSpace(options.Issuer))
                failures.Add($"{JwtOptions.SectionName}:{nameof(JwtOptions.Issuer)} is required.");

            if (string.IsNullOrWhiteSpace(options.Audience))
                failures.Add($"{JwtOptions.SectionName}:{nameof(JwtOptions.Audience)} is required.");

            if (string.IsNullOrWhiteSpace(options.Key))
                failures.Add(
                    $"{JwtOptions.SectionName}:{nameof(JwtOptions.Key)} is required. " +
                    "Set it with 'dotnet user-secrets set \"Jwt:Key\" \"<value>\"' in development " +
                    "or the Jwt__Key environment variable in production — never appsettings.json.");
            else if (Encoding.UTF8.GetByteCount(options.Key) < JwtOptions.MinKeyBytes)
                failures.Add(
                    $"{JwtOptions.SectionName}:{nameof(JwtOptions.Key)} must be at least " +
                    $"{JwtOptions.MinKeyBytes} bytes (256 bits) for HMAC-SHA256.");

            if (options.AccessTokenMinutes is < 1 or > 1440)
                failures.Add($"{JwtOptions.SectionName}:{nameof(JwtOptions.AccessTokenMinutes)} must be between 1 and 1440.");

            return failures.Count > 0
                ? ValidateOptionsResult.Fail(failures)
                : ValidateOptionsResult.Success;
        }
    }
}

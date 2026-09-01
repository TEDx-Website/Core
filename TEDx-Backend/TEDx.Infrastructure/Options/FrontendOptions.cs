using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace TEDx.Infrastructure.Options
{
    public sealed class FrontendOptions
    {
        public const string SectionName = "Frontend";

        [Required]
        [Url]
        public string BaseUrl { get; init; } = default!;

        public string ResetPasswordPath { get; init; } = "/reset-password";

        public string ConfirmEmailPath { get; init; } = "/confirm-email";
    }

    public sealed class FrontendOptionsValidator : IValidateOptions<FrontendOptions>
    {
        public ValidateOptionsResult Validate(string? name, FrontendOptions options)
        {
            var failures = new List<string>();

            if (string.IsNullOrWhiteSpace(options.BaseUrl))
            {
                failures.Add($"{FrontendOptions.SectionName}:{nameof(FrontendOptions.BaseUrl)} is required.");
            }
            else if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var baseUri)
                     || (baseUri.Scheme != Uri.UriSchemeHttp && baseUri.Scheme != Uri.UriSchemeHttps))
            {
                failures.Add(
                    $"{FrontendOptions.SectionName}:{nameof(FrontendOptions.BaseUrl)} must be an absolute http or https URL.");
            }

            if (string.IsNullOrWhiteSpace(options.ResetPasswordPath))
            {
                failures.Add(
                    $"{FrontendOptions.SectionName}:{nameof(FrontendOptions.ResetPasswordPath)} is required.");
            }

            if (string.IsNullOrWhiteSpace(options.ConfirmEmailPath))
            {
                failures.Add(
                    $"{FrontendOptions.SectionName}:{nameof(FrontendOptions.ConfirmEmailPath)} is required.");
            }

            return failures.Count > 0
                ? ValidateOptionsResult.Fail(failures)
                : ValidateOptionsResult.Success;
        }
    }
}

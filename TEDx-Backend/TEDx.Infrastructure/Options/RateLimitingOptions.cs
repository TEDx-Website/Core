using Microsoft.Extensions.Options;

namespace TEDx.Infrastructure.Configuration;

public sealed class RateLimitingOptions
{
    public const string SectionName = "RateLimiting";

    public bool Enabled { get; init; } = true;
    public bool TrustForwardedHeaders { get; init; }
    public IReadOnlyList<string> TrustedProxies { get; init; } = [];
    public IReadOnlyDictionary<string, RateLimitGroupOptions> Groups { get; init; }
        = new Dictionary<string, RateLimitGroupOptions>();
}

public sealed class RateLimitGroupOptions
{
    public int PermitLimit { get; init; } = 10;
    public int WindowSeconds { get; init; } = 60;
    public int QueueLimit { get; init; }
}

public sealed class RateLimitingOptionsValidator : IValidateOptions<RateLimitingOptions>
{
    public ValidateOptionsResult Validate(string? name, RateLimitingOptions options)
    {
        var failures = new List<string>();

        if (options.TrustForwardedHeaders && options.TrustedProxies.Count == 0)
        {
            failures.Add(
                $"{RateLimitingOptions.SectionName}:{nameof(RateLimitingOptions.TrustedProxies)} " +
                $"must name at least one proxy when " +
                $"{nameof(RateLimitingOptions.TrustForwardedHeaders)} is enabled.");
        }

        foreach (var proxy in options.TrustedProxies)
        {
            if (!System.Net.IPAddress.TryParse(proxy, out _))
            {
                failures.Add(
                    $"{RateLimitingOptions.SectionName}:{nameof(RateLimitingOptions.TrustedProxies)} " +
                    $"contains '{proxy}', which is not a valid IP address.");
            }
        }

        foreach (var (group, limits) in options.Groups)
        {
            var path = $"{RateLimitingOptions.SectionName}:{nameof(RateLimitingOptions.Groups)}:{group}";

            if (limits is null)
            {
                failures.Add($"{path} is missing its limits.");
                continue;
            }

            if (limits.PermitLimit < 1)
                failures.Add($"{path}:{nameof(RateLimitGroupOptions.PermitLimit)} must be at least 1.");

            if (limits.WindowSeconds < 1)
                failures.Add($"{path}:{nameof(RateLimitGroupOptions.WindowSeconds)} must be at least 1 second.");

            if (limits.QueueLimit < 0)
                failures.Add($"{path}:{nameof(RateLimitGroupOptions.QueueLimit)} must be non-negative.");
        }

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}

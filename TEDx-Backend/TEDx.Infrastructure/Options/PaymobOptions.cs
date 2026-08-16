using Microsoft.Extensions.Options;

namespace TEDx.Infrastructure.Options;

public sealed class PaymobOptions
{
    public const string SectionName = "Paymob";
    public string BaseUrl { get; init; } = "https://accept.paymob.com/api/";
    public string ApiKey { get; init; } = string.Empty;
    public string HmacSecret { get; init; } = string.Empty;
    public IReadOnlyList<int> IntegrationIds { get; init; } = [];
}

public sealed class PaymobOptionsValidator : IValidateOptions<PaymobOptions>
{
    public ValidateOptionsResult Validate(string? name, PaymobOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.ApiKey))
            failures.Add($"{PaymobOptions.SectionName}:{nameof(PaymobOptions.ApiKey)} is required.");

        if (string.IsNullOrWhiteSpace(options.HmacSecret))
            failures.Add($"{PaymobOptions.SectionName}:{nameof(PaymobOptions.HmacSecret)} is required.");

        if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _))
            failures.Add($"{PaymobOptions.SectionName}:{nameof(PaymobOptions.BaseUrl)} must be an absolute URL.");

        if (options.IntegrationIds.Count == 0)
            failures.Add($"{PaymobOptions.SectionName}:{nameof(PaymobOptions.IntegrationIds)} must contain at least one integration ID.");

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}

using Microsoft.Extensions.Options;

namespace TEDx.Infrastructure.Configuration;

public sealed class CloudinaryOptions
{
    public const string SectionName = "Cloudinary";

    public string CloudName { get; init; } = string.Empty;

    public string ApiKey { get; init; } = string.Empty;

    public string ApiSecret { get; init; } = string.Empty;

    public long MaxFileSizeBytes { get; init; } = 5 * 1024 * 1024;

    public IReadOnlyList<string> AllowedMimeTypes { get; init; } =
        ["image/jpeg", "image/png", "image/webp"];

    public string UploadFolder { get; init; } = "tedx";
}

public sealed class CloudinaryOptionsValidator : IValidateOptions<CloudinaryOptions>
{
    public ValidateOptionsResult Validate(string? name, CloudinaryOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.CloudName))
            failures.Add($"{CloudinaryOptions.SectionName}:{nameof(CloudinaryOptions.CloudName)} is required.");

        if (string.IsNullOrWhiteSpace(options.ApiKey))
            failures.Add($"{CloudinaryOptions.SectionName}:{nameof(CloudinaryOptions.ApiKey)} is required.");

        if (string.IsNullOrWhiteSpace(options.ApiSecret))
            failures.Add($"{CloudinaryOptions.SectionName}:{nameof(CloudinaryOptions.ApiSecret)} is required.");

        if (options.MaxFileSizeBytes <= 0)
            failures.Add($"{CloudinaryOptions.SectionName}:{nameof(CloudinaryOptions.MaxFileSizeBytes)} must be greater than zero.");

        if (options.AllowedMimeTypes.Count == 0)
            failures.Add($"{CloudinaryOptions.SectionName}:{nameof(CloudinaryOptions.AllowedMimeTypes)} must contain at least one MIME type.");

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}

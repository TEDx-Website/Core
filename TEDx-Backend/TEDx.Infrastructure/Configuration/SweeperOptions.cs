using Microsoft.Extensions.Options;

namespace TEDx.Infrastructure.Configuration;

public sealed class SweeperOptions
{
    public const string SectionName = "Sweeper";

    public int IntervalSeconds { get; init; } = 30;
    public int LockTimeoutMs { get; init; } = 5000;
}

public sealed class SweeperOptionsValidator : IValidateOptions<SweeperOptions>
{
    public ValidateOptionsResult Validate(string? name, SweeperOptions options)
    {
        var failures = new List<string>();

        if (options.IntervalSeconds < 5)
            failures.Add($"{SweeperOptions.SectionName}:{nameof(SweeperOptions.IntervalSeconds)} must be at least 5 seconds.");

        if (options.LockTimeoutMs < 0)
            failures.Add($"{SweeperOptions.SectionName}:{nameof(SweeperOptions.LockTimeoutMs)} must be non-negative.");

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}

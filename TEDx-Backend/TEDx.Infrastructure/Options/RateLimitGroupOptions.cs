namespace TEDx.Infrastructure.Options;

public sealed class RateLimitGroupOptions
{
    public int PermitLimit { get; init; } = 10;
    public int WindowSeconds { get; init; } = 60;
    public int QueueLimit { get; init; }
}

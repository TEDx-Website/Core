using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using TEDx.Api.Mapping;
using TEDx.Application.Common.Errors;
using TEDx.Infrastructure.Options;

namespace TEDx.Api.RateLimiting;

/// <summary>
/// <b>Window vs. Retry-After</b>
///
/// <b>Window</b> is a configuration value (e.g. 60 seconds) that defines the lifetime
/// of each rate-limit bucket before it resets.
///
/// <b>Retry-After</b> is a runtime value returned only when a request exceeds the rate
/// limit. It tells the client when it is safe to retry. With
/// <see cref="FixedWindowRateLimiter"/>, it reports the full configured window rather
/// than the actual time remaining in the current one.
/// </summary>
public static class RateLimitingServiceExtensions
{
    private static readonly RateLimitGroupOptions ConservativeDefault = new();

    public static IServiceCollection AddTedxRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(limiter =>
        {
            // Return 429 when a request exceeds the configured rate limit.
            limiter.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // When the rate limit is exceeded, return our standard API error response.
            limiter.OnRejected = WriteRateLimitedEnvelopeAsync;

            // Register a rate-limiting policy for each configured policy group.
            foreach (var group in RateLimitPolicies.All)
                limiter.AddPolicy(group, context => Partition(context, group));
        });

        services
            // Configure how forwarded headers (client IP/protocol) are trusted.
            .AddOptions<ForwardedHeadersOptions>()

            // Apply our proxy settings so the real client IP can be resolved safely.
            .Configure<IOptions<RateLimitingOptions>>(ConfigureForwardedHeaders);

        return services;
    }

    public static IApplicationBuilder UseTedxForwardedHeaders(this WebApplication app)
    {
        // Read the application's rate-limiting settings.
        var options = app.Services.GetRequiredService<IOptions<RateLimitingOptions>>().Value;

        // Enable forwarded headers only when the app is behind trusted proxies.
        if (options.TrustForwardedHeaders)
            app.UseForwardedHeaders();

        return app;
    }

    private static RateLimitPartition<string> Partition(HttpContext context, string group)
    {
        // Read the configured rate-limiting settings.
        var options = context.RequestServices
            .GetRequiredService<IOptions<RateLimitingOptions>>()
            .Value;

        // If rate limiting is turned off, allow all requests.
        if (!options.Enabled)
            return RateLimitPartition.GetNoLimiter($"{group}:disabled");

        // Get this group's limits (e.g. auth or auth-mail).
        var limits = ResolveLimits(options, group);

        return RateLimitPartition.GetFixedWindowLimiter(
            // Partition key: each group + client gets its own independent counter.
            $"{group}:{ResolveClientKey(context)}", 

            // Configure the fixed-window limiter for this partition.
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = limits.PermitLimit, // Maximum requests allowed in one time window.
                Window = TimeSpan.FromSeconds(limits.WindowSeconds), // Length of the rate-limit window.
                QueueLimit = limits.QueueLimit, // How many extra requests can wait in the queue. (Common in image upload)
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst, // Length of the rate-limit window.
                AutoReplenishment = true, // Allow all requests when rate limiting is globally disabled.
            });
    }

    private static RateLimitGroupOptions ResolveLimits(RateLimitingOptions options, string group)
        => options.Groups.TryGetValue(group, out var configured) && configured is not null
            ? configured
            : ConservativeDefault;

    private static string ResolveClientKey(HttpContext context)
    {
        // Get the client's IP address.
        var address = context.Connection.RemoteIpAddress;

        // Use a fallback bucket if no client IP is available.
        if (address is null)
            return "ip:unknown";

        // Convert IPv4-mapped IPv6 addresses (e.g. ::ffff:203.0.113.7) to plain IPv4 (203.0.113.7). 
        // They represent the same client, but without normalization the
        // app would treat them as different clients and create separate rate-limit buckets.
        if (address.IsIPv4MappedToIPv6)
            address = address.MapToIPv4();

        // IPv4 uses the full address. IPv6 uses only a stable prefix because
        // the temporary suffix may change for the same client over time.
        return address.AddressFamily == AddressFamily.InterNetworkV6
            ? $"ip6:{Ipv6SubscriberPrefix(address)}"
            : $"ip:{address}";
    }

    private static string Ipv6SubscriberPrefix(IPAddress address)
    {
        Span<byte> octets = stackalloc byte[16];

        if (!address.TryWriteBytes(octets, out var written) || written != 16)
            return address.ToString();

        return Convert.ToHexString(octets[..8]);
    }

    private static ValueTask WriteRateLimitedEnvelopeAsync(
        OnRejectedContext rejected,
        CancellationToken cancellationToken)
    {
        var context = rejected.HttpContext;

        LogRejection(context);

        if (context.Response.HasStarted)
            return ValueTask.CompletedTask;

        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;

        SetRetryAfter(rejected);

        var mapped = ErrorResultMapper.Map(
            [CommonErrors.RateLimited],
            context.Items["CorrelationId"] as string);

        return new ValueTask(
            context.Response.WriteAsJsonAsync(mapped.Body, cancellationToken));
    }

    private static void SetRetryAfter(OnRejectedContext rejected)
    {
        // If the request was rate-limited, read the retry delay reported by the rate
        // limiter. Round up so the client never retries too early.
        var seconds = rejected.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)
            ? (int)Math.Ceiling(retryAfter.TotalSeconds)
            : 0;

        // Tell the client when it is safe to retry. Round up and never return less
        // than 1 second to avoid retrying immediately into another 429 response.
        rejected.HttpContext.Response.Headers.RetryAfter =
            Math.Max(seconds, 1).ToString(NumberFormatInfo.InvariantInfo);
    }

    private static void LogRejection(HttpContext context)
    {
        // Logged at Warning: a tripped auth limiter is either an abuse signal worth alerting on or
        // a limit tuned too tight. Either way an operator needs to see it without turning on debug.
        context.RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("TEDx.Api.RateLimiting")
            .LogWarning(
                "Rate limit tripped for {Method} {Path} from {ClientKey}.",
                context.Request.Method,
                context.Request.Path.Value,
                ResolveClientKey(context));
    }

    private static void ConfigureForwardedHeaders(
        ForwardedHeadersOptions forwarded,
        IOptions<RateLimitingOptions> rateLimiting)
    {
        var options = rateLimiting.Value;

        // Trust the proxy to forward the original client IP and protocol.
        forwarded.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

        // Remove the framework's default trusted proxies (e.g. 127.0.0.1). We'll add our own below.
        forwarded.KnownProxies.Clear();
        forwarded.KnownIPNetworks.Clear();

        foreach (var proxy in options.TrustedProxies)
        {
            // Add only valid IP addresses as trusted proxies.
            if (IPAddress.TryParse(proxy, out var parsed))
                forwarded.KnownProxies.Add(parsed);
        }

        // Trust only the expected number of proxy hops to resolve the real client IP safely.
        forwarded.ForwardLimit = Math.Max(forwarded.KnownProxies.Count, 1);
    }
}

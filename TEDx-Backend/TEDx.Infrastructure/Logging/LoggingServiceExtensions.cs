using Microsoft.Extensions.Hosting;
using Serilog;
using TEDx.Infrastructure.Logging;

namespace Microsoft.Extensions.DependencyInjection;

public static class LoggingServiceExtensions
{
    /// <summary>
    /// Configures Serilog entirely from <c>IConfiguration</c> (the "Serilog" section),
    /// so sinks, levels, and destinations are swappable without code changes.
    /// Only the pieces that must be code — LogContext + the sensitive-data enricher — are wired here.
    /// </summary>
    public static IHostBuilder AddTedxSerilog(this IHostBuilder builder) =>
        builder.UseSerilog((ctx, services, config) =>
        {
            config
                .ReadFrom.Configuration(ctx.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .Enrich.With<SensitivePropertyEnricher>();
        });
}

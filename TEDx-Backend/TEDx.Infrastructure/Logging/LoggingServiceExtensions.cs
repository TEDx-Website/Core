using Microsoft.Extensions.Hosting;
using Serilog;
using TEDx.Infrastructure.Logging;

namespace Microsoft.Extensions.DependencyInjection;

public static class LoggingServiceExtensions
{
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

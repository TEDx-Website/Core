using Microsoft.Extensions.Configuration;
using TEDx.Application.Ticketing.Payments;
using TEDx.Infrastructure.Payments;

namespace Microsoft.Extensions.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTedxOptions(configuration);

        services.AddScoped<IPaymobClient, PaymobClient>();

        return services;
    }
}

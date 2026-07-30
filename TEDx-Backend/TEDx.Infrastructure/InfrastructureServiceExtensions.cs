using Microsoft.Extensions.Configuration;
using TEDx.Application.Ticketing.Payments;
using TEDx.Infrastructure.Configuration;
using TEDx.Infrastructure.Email;
using TEDx.Infrastructure.Payments;
using TEDx.Application.Common.Interfaces;
namespace Microsoft.Extensions.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTedxOptions(configuration);

        services.AddScoped<IPaymobClient, PaymobClient>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();


        return services;
    }
}

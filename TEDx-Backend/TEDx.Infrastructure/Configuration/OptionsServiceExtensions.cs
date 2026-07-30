using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TEDx.Infrastructure.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class OptionsServiceExtensions
{
    public static IServiceCollection AddTedxOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatedOptions<PaymobOptions, PaymobOptionsValidator>(configuration, PaymobOptions.SectionName);

        return services;
    }

    private static IServiceCollection AddValidatedOptions<TOptions, TValidator>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
        where TOptions : class
        where TValidator : class, IValidateOptions<TOptions>
    {
        services.AddSingleton<IValidateOptions<TOptions>, TValidator>();

        services
            .AddOptions<TOptions>()
            .Bind(configuration.GetSection(sectionName));
        //  .ValidateOnStart();

        return services;
    }
}

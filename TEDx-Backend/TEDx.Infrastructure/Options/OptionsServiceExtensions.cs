using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TEDx.Infrastructure.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class OptionsServiceExtensions
{
    public static IServiceCollection AddTedxOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatedOptions<PaymobOptions, PaymobOptionsValidator>(configuration, PaymobOptions.SectionName);
        services.AddValidatedOptions<SmtpOptions, SmtpOptionsValidator>(configuration, SmtpOptions.SectionName);
        services.AddValidatedOptions<CloudinaryOptions, CloudinaryOptionsValidator>(configuration, CloudinaryOptions.SectionName);
        services.AddValidatedOptions<SweeperOptions, SweeperOptionsValidator>(configuration, SweeperOptions.SectionName);
        services.AddValidatedOptions<IdentityPolicyOptions,IdentityPolicyOptionsValidator>(configuration,IdentityPolicyOptions.SectionName);
        services.AddValidatedOptions<JwtOptions, JwtOptionsValidator>(configuration, JwtOptions.SectionName);
        services.AddValidatedOptions<RateLimitingOptions, RateLimitingOptionsValidator>(configuration, RateLimitingOptions.SectionName);
        services.AddValidatedOptions<FrontendOptions, FrontendOptionsValidator>(configuration, FrontendOptions.SectionName);

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
            //.ValidateOnStart();

        return services;
    }
}

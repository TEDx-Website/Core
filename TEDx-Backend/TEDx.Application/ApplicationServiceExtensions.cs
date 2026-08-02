using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TEDx.Application.Common.Behaviors;

namespace TEDx.Application;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = typeof(ApplicationServiceExtensions).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // Pipeline order: Logging → Validation → Authorization → Handler.
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        return services;
    }
}

using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TEDx.Application.Common.Behaviors;
using TEDx.Application.Identity.Service;
using TEDx.Application.Ticketing.Availability;

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
        services.AddScoped<IMyProfileService, MyProfileService>();

        services.AddScoped<IEventSeatAvailabilityReader, EventSeatAvailabilityReader>();

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        return services;
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TEDx.Application.Common.Interfaces;
using TEDx.Application.Ticketing.Payments;
using TEDx.Domain.Identity.Entities;
using TEDx.Infrastructure.Common;
using TEDx.Infrastructure.Configuration;
using TEDx.Infrastructure.Email;
using TEDx.Infrastructure.Identity;
using TEDx.Infrastructure.Payments;
using TEDx.Infrastructure.Persistence;
using TEDx.Infrastructure.Persistence.Interceptors;
using TEDx.Infrastructure.Persistence.Seeding;
namespace Microsoft.Extensions.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTedxOptions(configuration);

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddSingleton<IClock, SystemClock>();

        services.AddScoped<AuditInterceptor>();

        services.AddDbContext<AppDbContext>((sp, options) =>
            options
                .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(sp.GetRequiredService<AuditInterceptor>()));

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddIdentityCore<User>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>();

        services.AddScoped<AdminSeeder>();

        services.AddScoped<IPaymobClient, PaymobClient>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();

        return services;
    }
}

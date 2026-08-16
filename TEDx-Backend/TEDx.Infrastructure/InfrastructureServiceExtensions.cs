using CloudinaryDotNet;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TEDx.Application.Common.Interfaces;
using TEDx.Application.Ticketing.Payments;
using TEDx.Domain.Identity.Entities;
using TEDx.Infrastructure.BackgroundJobs;
using TEDx.Infrastructure.Common;
using TEDx.Infrastructure.Options;
using TEDx.Infrastructure.Email;
using TEDx.Infrastructure.Identity;
using TEDx.Infrastructure.Media;
using TEDx.Infrastructure.Payments;
using TEDx.Infrastructure.Persistence;
using TEDx.Infrastructure.Persistence.Interceptors;
using TEDx.Infrastructure.Persistence.Seeding;

namespace Microsoft.Extensions.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    private const string EmailConfirmationProvider = "email_confirmation";

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTedxOptions(configuration);

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<ITrackAccessReader, TrackAccessReader>();
        services.AddSingleton<IClock, SystemClock>();

        services.AddScoped<AuditInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
            options
                .UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name))
                .AddInterceptors(sp.GetRequiredService<AuditInterceptor>())
                // The Identity claim/login/token entities are deliberately ignored
                // (see ApplicationDbContext.OnModelCreating) — the warning is expected.
                .ConfigureWarnings(w =>
                    w.Ignore(CoreEventId.MappedEntityTypeIgnoredWarning)));


        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<AdminSeeder>();

        services.AddScoped<IPaymobClient, PaymobClient>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();

        services.AddSingleton(sp =>
        {
            var cloudinary = sp.GetRequiredService<IOptions<CloudinaryOptions>>().Value;

            var account = new Account(
                cloudinary.CloudName,
                cloudinary.ApiKey,
                cloudinary.ApiSecret);

            return new Cloudinary(account) { Api = { Secure = true } };
        });

        services.AddScoped<IImageUploadService, CloudinaryImageUploadService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IUserAccountService, UserAccountService>();
        services.AddSingleton<IAuthLinkBuilder, AuthLinkBuilder>();
        services.AddDataProtection();

        services.AddHostedService<OutboxAndHoldExpirySweeper>();

        var policy = configuration.GetSection(IdentityPolicyOptions.SectionName)
                                  .Get<IdentityPolicyOptions>() ?? new IdentityPolicyOptions();

        services.AddIdentityCore<User>(options =>
        {
            options.Password.RequiredLength = policy.PasswordMinLength;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = false;


            options.Lockout.MaxFailedAccessAttempts = policy.MaxFailedAttempts;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(policy.LockoutMinutes);
            options.Lockout.AllowedForNewUsers = true;

            options.User.RequireUniqueEmail = true;

            options.Tokens.EmailConfirmationTokenProvider = EmailConfirmationProvider;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders()
        .AddTokenProvider<EmailConfirmationTokenProvider>(EmailConfirmationProvider);

        services.Configure<DataProtectionTokenProviderOptions>(o =>
            o.TokenLifespan = TimeSpan.FromHours(policy.ResetTokenHours));

        services.Configure<EmailConfirmationTokenProviderOptions>(o =>
            o.TokenLifespan = TimeSpan.FromHours(policy.ConfirmTokenHours));

        return services;
    }
}


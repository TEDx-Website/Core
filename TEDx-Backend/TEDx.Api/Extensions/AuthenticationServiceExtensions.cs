using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TEDx.Api.Mapping;
using TEDx.Application.Common;
using TEDx.Domain.Common;
using TEDx.Infrastructure.Configuration;

namespace TEDx.Api.Extensions;

public static class AuthenticationServiceExtensions
{
    private const string AuthLoggerCategory = "TEDx.Api.Authentication";

    public static IServiceCollection AddTedxAuthentication(this IServiceCollection services)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();

        services
            .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>(ConfigureBearer);

        services.AddAuthorization();

        return services;
    }

    private static void ConfigureBearer(JwtBearerOptions bearer, IOptions<JwtOptions> jwtOptions)
    {
        var jwt = jwtOptions.Value;

        bearer.MapInboundClaims = true;

        bearer.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,

            ValidateAudience = true,
            ValidAudience = jwt.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,

            NameClaimType = ClaimTypes.NameIdentifier,
            RoleClaimType = ClaimTypes.Role,
        };

        bearer.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = OnAuthenticationFailed,
            OnChallenge = OnChallenge,
            OnForbidden = OnForbidden,
        };
    }

    private static Task OnAuthenticationFailed(AuthenticationFailedContext context)
    {
        context.HttpContext.RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger(AuthLoggerCategory)
            .LogInformation("Bearer authentication failed: {Reason}.", context.Exception.GetType().Name);

        if (context.Exception is SecurityTokenExpiredException)
            context.Response.Headers.Append("X-Token-Expired", "true");

        return Task.CompletedTask;
    }

    private static Task OnChallenge(JwtBearerChallengeContext context)
    {
        context.HandleResponse();

        // RFC 6750 still wants the challenge header even though we own the body.
        context.Response.Headers.WWWAuthenticate = context.AuthenticateFailure is null
            ? "Bearer"
            : "Bearer error=\"invalid_token\"";

        return WriteEnvelopeAsync(context.HttpContext, Errors.Unauthenticated);
    }

    private static Task OnForbidden(ForbiddenContext context)
        => WriteEnvelopeAsync(context.HttpContext, Errors.Forbidden);

    private static Task WriteEnvelopeAsync(HttpContext context, Error error)
    {
        if (context.Response.HasStarted)
            return Task.CompletedTask;

        var mapped = ErrorResultMapper.Map([error], context.Items["CorrelationId"] as string);

        context.Response.StatusCode = mapped.StatusCode;
        context.Response.ContentType = "application/json";

        return context.Response.WriteAsJsonAsync(mapped.Body);
    }
}

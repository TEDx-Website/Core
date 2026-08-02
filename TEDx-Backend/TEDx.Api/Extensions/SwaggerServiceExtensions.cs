using Microsoft.OpenApi.Models;

namespace TEDx.Api.Extensions;

public static class SwaggerServiceExtensions
{
    private const string BearerScheme = "Bearer";

    public static IServiceCollection AddTedxSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(BearerScheme, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Paste the raw access token only, without the \"Bearer \" prefix.",
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = BearerScheme,
                    },
                }] = Array.Empty<string>(),
            });
        });

        return services;
    }
}

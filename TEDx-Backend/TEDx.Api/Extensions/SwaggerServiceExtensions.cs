using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace TEDx.Api.Extensions;

public static class SwaggerServiceExtensions
{
    private const string BearerScheme = "Bearer";

    private const string DarkThemePath = "/swagger-ui/tedx-dark.css";

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

    public static void ConfigureTedxSwaggerUI(this SwaggerUIOptions options)
    {
        options.DocumentTitle = "TEDx API";
        options.InjectStylesheet(DarkThemePath);

        options.DocExpansion(DocExpansion.List);
        options.DefaultModelsExpandDepth(0);
        options.EnableDeepLinking();
        options.DisplayRequestDuration();
    }
}

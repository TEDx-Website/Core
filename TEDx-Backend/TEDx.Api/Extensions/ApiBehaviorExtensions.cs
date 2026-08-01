using Microsoft.AspNetCore.Mvc;
using TEDx.Api.Common.Respones;

namespace TEDx.Api.Extensions;

public static class ApiBehaviorExtensions
{
    /// <summary>
    /// Configures API behavior to ensure model validation errors (400 Bad Request)
    /// return our standard { success, data, error } envelope format.
    /// </summary>
    public static IServiceCollection AddCustomApiBehavior(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            // Override the default validation response factory
            options.InvalidModelStateResponseFactory = context =>
            {
                // Extract field errors and convert keys to camelCase
                var fieldErrors = context.ModelState
                    .Where(kvp => kvp.Value is not null && kvp.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => ToCamelCase(kvp.Key),
                        kvp => kvp.Value!.Errors
                            .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage)
                                ? "The value is invalid."
                                : e.ErrorMessage)
                            .ToArray());

                // Build the standard response envelope expected by the SPA
                var response = ApiResponse<object>.FailureResult(new ApiErrorResponse
                {
                    Code = "BAD_REQUEST",
                    Message = "The request is malformed or contains invalid values.",
                    FieldErrors = fieldErrors.Count > 0 ? fieldErrors : null,
                    TraceId = context.HttpContext.Items["CorrelationId"] as string,
                });

                // Return a 400 Bad Request
                return new BadRequestObjectResult(response);
            };
        });

        return services;
    }

    private static string ToCamelCase(string field)
    {
        if (string.IsNullOrEmpty(field) || char.IsLower(field[0]))
            return field;

        return char.ToLowerInvariant(field[0]) + field[1..];
    }
    public static IServiceCollection AddApiBehavior(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        var methos = configuration.GetSection("Cors:AllowedMethods").Get<string[]>();
        var headers = configuration.GetSection("Cors:AllowedHeaders").Get<string[]>();
        services.AddCors(options =>
        {
            options.AddPolicy("DefaultCorsPolicy", policy =>
            {
                policy
                    .WithMethods(methos!)
                    .WithHeaders(headers!)
                    .WithOrigins(
                        configuration.GetSection("Cors:AllowedOrigins")
                                     .Get<string[]>()!);
            });
        });

        return services;
    }
}

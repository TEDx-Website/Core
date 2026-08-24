using Microsoft.AspNetCore.Mvc;
using TEDx.Api.Common.Responses;
using TEDx.Application.Common.Errors;

namespace TEDx.Api.Extensions;

public static class ApiBehaviorExtensions
{
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
                var malformed = context.ModelState // Check if the model state contains any errors with exceptions (if T -> BadRequest bc invalid Values , F -> Validation error bc missing data in JSON response) 
                                    .Any(kvp => kvp.Value is not null
                                                && kvp.Value.Errors.Any(e => e.Exception is not null));

                // Build the standard response envelope expected by the SPA
                var response = ApiResponse<object>.FailureResult(new ApiError
                {
                    Code = malformed
                        ? "BAD_REQUEST" // Malformed request, invalid values
                        : CommonErrors.ValidationError.Code, // Validation error, missing data in JSON response
                    Message = malformed
                        ? "The request is malformed or contains invalid values."
                        : CommonErrors.ValidationError.Description,

                    // §0.2: fieldErrors belongs to input-validation failures only.
                    // !malformed -> Validation error, missing data in JSON response ,
                    // bc malformed -> Malformed request, invalid values, invalid values can't be filed validation

                    FieldErrors = !malformed && fieldErrors.Count > 0 ? fieldErrors : null,
                    TraceId = context.HttpContext.Items["CorrelationId"] as string,
                });
                //return new BadRequestObjectResult(response); // always returns 400 only, but we want to return 422 for validation errors
                return new ObjectResult(response)
                {
                    StatusCode = malformed
                        ? StatusCodes.Status400BadRequest
                        : StatusCodes.Status422UnprocessableEntity,
                };
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

        var methods = configuration.GetSection("Cors:AllowedMethods").Get<string[]>();
        var headers = configuration.GetSection("Cors:AllowedHeaders").Get<string[]>();
        services.AddCors(options =>
        {
            options.AddPolicy("DefaultCorsPolicy", policy =>
            {
                policy
                    .WithMethods(methods!)
                    .WithHeaders(headers!)
                    .WithOrigins(
                        configuration.GetSection("Cors:AllowedOrigins")
                                     .Get<string[]>()!);
            });
        });

        return services;
    }
}

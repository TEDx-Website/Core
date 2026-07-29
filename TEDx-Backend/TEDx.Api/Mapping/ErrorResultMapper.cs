using Microsoft.AspNetCore.Http;
using TEDx.Api.Common.Respones;
using TEDx.Domain.Common;

namespace TEDx.Api.Mapping;

public static class ErrorResultMapper
{
    public const string InternalErrorCode = "INTERNAL_SERVER_ERROR";
    public const string InternalErrorMessage = "An unexpected error occurred.";

    public readonly record struct MappedError(int StatusCode, ApiResponse<object> Body);

    public static MappedError Map(IReadOnlyList<Error>? errors, string? traceId)
    {
        if (errors is null || errors.Count == 0)
        {
            var fallback = ApiResponse<object>.FailureResult(new ApiErrorResponse
            {
                Code = InternalErrorCode,
                Message = InternalErrorMessage,
                TraceId = traceId,
            });
            return new MappedError(StatusCodes.Status500InternalServerError, fallback);
        }

        var first = errors[0];

        var apiError = new ApiErrorResponse
        {
            Code = first.Code,
            Message = first.Description,
            TraceId = traceId,
            FieldErrors = BuildFieldErrors(errors),
        };

        return new MappedError(
            ToStatusCode(first.Type),
            ApiResponse<object>.FailureResult(apiError));
    }


    public static int ToStatusCode(ErrorType type) => type switch
    {
        ErrorType.Validation => StatusCodes.Status422UnprocessableEntity,   // 422
        ErrorType.Business => StatusCodes.Status422UnprocessableEntity,     // 422
        ErrorType.BadRequest => StatusCodes.Status400BadRequest,            // 400
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,       // 401
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,             // 403
        ErrorType.NotFound => StatusCodes.Status404NotFound,               // 404
        ErrorType.Conflict => StatusCodes.Status409Conflict,               // 409
        ErrorType.RateLimited => StatusCodes.Status429TooManyRequests,     // 429
        // Unexpected / None / anything unmapped is an incident, not a client error.
        _ => StatusCodes.Status500InternalServerError,                      // 500
    };


    private static Dictionary<string, string[]>? BuildFieldErrors(IReadOnlyList<Error> errors)
    {
        var fieldErrors = errors
            .Where(e => e.Type == ErrorType.Validation && !string.IsNullOrWhiteSpace(e.Field))
            .GroupBy(e => ToCamelCase(e.Field!))
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.Description).ToArray());

        return fieldErrors.Count > 0 ? fieldErrors : null;
    }

    private static string ToCamelCase(string field)
    {
        if (string.IsNullOrEmpty(field) || char.IsLower(field[0]))
            return field;

        return char.ToLowerInvariant(field[0]) + field[1..];
    }
}

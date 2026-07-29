using TEDx.Domain.Common;

namespace TEDx.Application.Common;
public static partial class Errors
{
    public static readonly Error ValidationError =
     Error.Validation(
         "VALIDATION_ERROR",
         "One or more fields are invalid.");

    public static readonly Error NotFound =
        Error.NotFound(
            "NOT_FOUND",
            "The requested resource was not found.");

    public static readonly Error ConcurrencyConflict =
        Error.Conflict(
            "CONCURRENCY_CONFLICT",
            "The resource was modified by another request.");

    public static readonly Error RateLimited =
        Error.RateLimited(
            "RATE_LIMITED",
            "Too many requests. Try again later.");

    public static readonly Error ConfirmationRequired =
        Error.Conflict(
            "CONFIRMATION_REQUIRED",
            "This action requires explicit confirmation.");

    public static readonly Error IllegalStatusTransition =
        Error.Conflict(
            "ILLEGAL_STATUS_TRANSITION",
            "This status change is not allowed.");
}

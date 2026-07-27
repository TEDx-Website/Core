using TEDx.Domain.Common;

namespace TEDx.Application.Common;

public static partial class Errors
{
    public static readonly Error ValidationError =
        new("VALIDATION_ERROR", "One or more fields are invalid.", ErrorType.Validation);

    public static readonly Error NotFound =
        new("NOT_FOUND", "The requested resource was not found.", ErrorType.NotFound);

    public static readonly Error ConcurrencyConflict =
        new("CONCURRENCY_CONFLICT", "The resource was modified by another request.", ErrorType.Conflict);

    public static readonly Error RateLimited =
        new("RATE_LIMITED", "Too many requests. Try again later.", ErrorType.Business);

    public static readonly Error ConfirmationRequired =
        new("CONFIRMATION_REQUIRED", "This action requires explicit confirmation.", ErrorType.Conflict);

    // State-transition family (audit-Issue-30): 409 = state/concurrency.
    public static readonly Error IllegalStatusTransition =
        new("ILLEGAL_STATUS_TRANSITION", "This status change is not allowed.", ErrorType.Conflict);
}

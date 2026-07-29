namespace TEDx.Domain.Common;

public enum ErrorType
{
    None,
    Validation,
    BadRequest,
    NotFound,
    Conflict,
    Business,
    RateLimited,
    Unauthorized,
    Unexpected,
    Forbidden,
}

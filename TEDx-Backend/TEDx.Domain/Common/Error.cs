namespace TEDx.Domain.Common;

public readonly record struct Error
{
    private Error(string code, string description, ErrorType type,
        string? field = null,
        IReadOnlyDictionary<string, object?>? metaData = null)
    {
        Code = code;
        Description = description;
        Type = type;
        Field = field;
        MetaData = metaData;
    }


    public string Code { get; }
    public string Description { get; }
    public ErrorType Type { get; }

    public string? Field { get; }
    public IReadOnlyDictionary<string, object?>? MetaData { get; } // extra space to store additional information about the error

    // Factory methods for creating different types of errors
    public static Error Validation(string code = nameof(Validation),
        string description = "Validation error",
        string? field = null,
        IReadOnlyDictionary<string, object?>? metaData = null)
        => new(code, description, ErrorType.Validation, field, metaData);

    public static Error BadRequest(string code = nameof(BadRequest),
        string description = "Bad request",
        IReadOnlyDictionary<string, object?>? metaData = null)
        => new(code, description, ErrorType.BadRequest, metaData: metaData);

    public static Error NotFound(string code = nameof(NotFound),
        string description = "Not found error",
        IReadOnlyDictionary<string, object?>? metaData = null)
        => new(code, description, ErrorType.NotFound, metaData: metaData);

    public static Error Unauthorized(string code = nameof(Unauthorized),
        string description = "Unauthorized error",
        IReadOnlyDictionary<string, object?>? metaData = null)
        => new(code, description, ErrorType.Unauthorized, metaData: metaData);

    public static Error Forbidden(string code = nameof(Forbidden),
        string description = "Forbidden error",
        IReadOnlyDictionary<string, object?>? metaData = null)
        => new(code, description, ErrorType.Forbidden, metaData: metaData);

    public static Error Conflict(string code = nameof(Conflict),
        string description = "Conflict error",
        IReadOnlyDictionary<string, object?>? metaData = null)
        => new(code, description, ErrorType.Conflict, metaData: metaData);

    public static Error RateLimited(string code = nameof(RateLimited),
        string description = "Too many requests",
        IReadOnlyDictionary<string, object?>? metaData = null)
        => new(code, description, ErrorType.RateLimited, metaData: metaData);

    public static Error Unexpected(string code = nameof(Unexpected),
        string description = "Unexpected error.",
        IReadOnlyDictionary<string, object?>? metaData = null)
        => new(code, description, ErrorType.Unexpected, metaData: metaData);

    public static Error Business(string code = nameof(Business),
        string description = "Business rule violation.",
        IReadOnlyDictionary<string, object?>? metaData = null)
        => new(code, description, ErrorType.Business, metaData: metaData);

    public static Error None(string code = nameof(None),
        string description = "No error.",
        IReadOnlyDictionary<string, object?>? metaData = null)
        => new(code, description, ErrorType.None, metaData: metaData);

    public static Error Failure(string code = nameof(Failure),
    string description = "Failure error.",
    IReadOnlyDictionary<string, object?>? metaData = null)
    => new(code, description, ErrorType.Failure, metaData: metaData);
}

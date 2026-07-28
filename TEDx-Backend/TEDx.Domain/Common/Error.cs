namespace TEDx.Domain.Common;

public readonly record struct Error
{
    private Error(string code, string description, ErrorType type,
        IReadOnlyDictionary<string, object?>? metaData = null)
    {
        Code = code;
        Description = description;
        Type = type;
        MetaData = metaData;
    }

    public string Code { get; }
    public string Description { get; }
    public ErrorType Type { get; }
    public IReadOnlyDictionary<string, object?>? MetaData { get; }

    public static Error Validation(string code = nameof(Validation),
        string description = "Validation error",
        IReadOnlyDictionary<string, object?>? metaData = null)
        => new(code, description, ErrorType.Validation, metaData);

    public static Error NotFound(string code = nameof(NotFound),
        string description = "Not found error",
        IReadOnlyDictionary<string, object?>? metaData = null)
        => new(code, description, ErrorType.NotFound, metaData);

    public static Error Unauthorized(string code = nameof(Unauthorized),
        string description = "Unauthorized error",
        IReadOnlyDictionary<string, object?>? metaData = null)
        => new(code, description, ErrorType.Unauthorized, metaData);

    public static Error Forbidden(string code = nameof(Forbidden),
        string description = "Forbidden error",
        IReadOnlyDictionary<string, object?>? metaData = null)
        => new(code, description, ErrorType.Forbidden, metaData);

    public static Error Conflict(string code = nameof(Conflict),
        string description = "Conflict error",
        IReadOnlyDictionary<string, object?>? metaData = null)
        => new(code, description, ErrorType.Conflict, metaData);

    public static Error Unexpected(string code = nameof(Unexpected),
        string description = "Unexpected error.",
        IReadOnlyDictionary<string, object?>? metaData = null)
        => new(code, description, ErrorType.Unexpected, metaData);

    public static Error Business(string code = nameof(Business),
        string description = "Business error.",
        IReadOnlyDictionary<string, object?>? metaData = null)
        => new(code, description, ErrorType.Business, metaData);

    public static Error None(string code = nameof(None),
        string description = "None error.",
        IReadOnlyDictionary<string, object?>? metaData = null)
        => new(code, description, ErrorType.None, metaData);
}


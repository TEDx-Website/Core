namespace TEDx.Domain.Common;

public readonly record struct Error(
    string Code,
    string Message,
    ErrorType Type
);
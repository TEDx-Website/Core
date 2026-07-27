namespace TEDx.Domain.Common;

public readonly record struct Error(string Code, string Message, ErrorType Type, string? Field = null)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Business);
}

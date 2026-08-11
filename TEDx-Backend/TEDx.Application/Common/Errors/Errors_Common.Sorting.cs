using TEDx.Application.Common.Pagination;
using TEDx.Domain.Common;

namespace TEDx.Application.Common.Errors;

public static partial class Errors_Common
{
    public const string SortParameterName = "sort";
    private const int MaxEchoedLength = 40;

    public static Error UnknownSortField(string? field, IEnumerable<string> allowedFields)
        => Error.Validation(
            ValidationError.Code,
            $"Sort field '{Echo(field)}' is not supported. Allowed fields: {string.Join(", ", allowedFields)}.",
            SortParameterName);

    public static Error MalformedSort(string? sort)
        => Error.Validation(
            ValidationError.Code,
            $"Sort '{Echo(sort)}' is malformed. Expected 'field' or "
            + $"'field{SortSyntax.Separator}{SortSyntax.Ascending}|{SortSyntax.Descending}'.",
            SortParameterName);

    private static string Echo(string? value)
    {
        var text = value ?? string.Empty;
        return text.Length <= MaxEchoedLength
            ? text
            : string.Concat(text.AsSpan(0, MaxEchoedLength), "…");
    }
}

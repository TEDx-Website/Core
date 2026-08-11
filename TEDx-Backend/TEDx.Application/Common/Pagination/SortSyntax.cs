namespace TEDx.Application.Common.Pagination;

public enum SortDirection
{
    Ascending = 0,
    Descending = 1,
}

public static class SortSyntax
{
    public const string Ascending = "asc";
    public const string Descending = "desc";
    public const char Separator = ':';
}

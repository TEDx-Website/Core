namespace TEDx.Application.Common.Pagination;

public readonly record struct PageRequest
{
    private PageRequest(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }

    public int Page { get; }
    public int PageSize { get; }
    public int Skip => (Page - 1) * PageSize;
    public int Take => PageSize;

    public static PageRequest From(int? page, int? pageSize)
        => new(NormalizePage(page), NormalizePageSize(pageSize));

    private static int NormalizePage(int? page)
        => page is null || page.Value < PaginationDefaults.MinPage
            ? PaginationDefaults.MinPage
            : page.Value;

    private static int NormalizePageSize(int? pageSize)
        => pageSize is null
            ? PaginationDefaults.DefaultPageSize
            : Math.Clamp(pageSize.Value, PaginationDefaults.MinPageSize, PaginationDefaults.MaxPageSize);
}

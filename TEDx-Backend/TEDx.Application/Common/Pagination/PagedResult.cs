namespace TEDx.Application.Common.Pagination;

public sealed record PagedResult<T>
{
    private PagedResult(IReadOnlyList<T> items, int page, int pageSize, int totalItems)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalItems = totalItems;
        TotalPages = CalculateTotalPages(totalItems, pageSize);
    }

    public IReadOnlyList<T> Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalItems { get; }
    public int TotalPages { get; }

    public static PagedResult<T> Create(IReadOnlyList<T> items, PagedRequest page, int totalItems)
        => new(items, page.Page, page.PageSize, totalItems);

    public static PagedResult<T> Empty(PagedRequest page)
        => new([], page.Page, page.PageSize, 0);

    public PagedResult<TOut> Map<TOut>(Func<T, TOut> selector)
        => new([.. Items.Select(selector)], Page, PageSize, TotalItems);

    private static int CalculateTotalPages(int totalItems, int pageSize)
        => totalItems <= 0 || pageSize <= 0
            ? 0
            : (int)Math.Ceiling(totalItems / (double)pageSize);
}

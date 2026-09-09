using Microsoft.EntityFrameworkCore;

namespace TEDx.Application.Common.Pagination;

public static class PagedQueryableExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> source,
        PagedRequest page,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        var totalItems = await source.CountAsync(cancellationToken);
        if (totalItems == 0 || page.Skip >= totalItems)
        {
            return PagedResult<T>.Create([], page, totalItems);
        }

        var items = await source
            .Skip(page.Skip)
            .Take(page.Take)
            .ToListAsync(cancellationToken);

        return PagedResult<T>.Create(items, page, totalItems);
    }
}

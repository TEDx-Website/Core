using TEDx.Application.Common.Pagination;
using TEDx.Domain.Ticketing.Entities;

namespace TEDx.Application.Ticketing.Queries;

public static class EventSorting
{
    public static readonly SortWhitelist<Event> Admin = new SortWhitelist<Event>()
        .Allow("startsAtUtc", e => e.StartAtUtc)
        .Allow("titleEn", e => e.TitleEn)
        .Allow("createdAt", e => e.CreatedAtUtc)
        .TieBreakBy(e => e.Id)
        .WithDefault("startsAtUtc", SortDirection.Descending);

    public static readonly SortWhitelist<Event> Public =
        new SortWhitelist<Event>()
            .Allow("startsAtUtc", e => e.StartAtUtc)
            .Allow("titleEn", e => e.TitleEn)
            .TieBreakBy(e => e.Id)
            .WithDefault("startsAtUtc", SortDirection.Ascending);
}

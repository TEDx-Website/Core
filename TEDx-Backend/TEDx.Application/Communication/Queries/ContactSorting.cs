using TEDx.Application.Common.Pagination;
using TEDx.Domain.Communication.Entities;

namespace TEDx.Application.Communication.Queries;

public static class ContactSorting
{
    public static readonly SortWhitelist<ContactMessage> Admin =
        new SortWhitelist<ContactMessage>()
            .Allow("createdAt", m => m.CreatedAtUtc)
            .TieBreakBy(m => m.Id)
            .WithDefault("createdAt", SortDirection.Descending);
}

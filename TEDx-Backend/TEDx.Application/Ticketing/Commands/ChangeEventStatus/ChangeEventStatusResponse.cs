using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Application.Ticketing.DTOs
{
    public sealed record ChangeEventStatusDTO(
        EventStatus Status,
        string RowVersion);
}

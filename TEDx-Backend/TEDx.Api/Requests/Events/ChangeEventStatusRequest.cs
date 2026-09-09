using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Api.Requests.Events
{
    public sealed record ChangeEventStatusRequest(EventStatus Status);
}

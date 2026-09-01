using TEDx.Domain.Communication.Enums;

namespace TEDx.Application.Ticketing.Commands.UpdateContactStatus
{
    public sealed record UpdateContactStatusResponse(
        Guid Id,
        ContactStatus Status);
}

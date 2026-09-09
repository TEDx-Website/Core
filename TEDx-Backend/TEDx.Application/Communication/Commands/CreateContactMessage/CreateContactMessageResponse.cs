using TEDx.Domain.Communication.Enums;

namespace TEDx.Application.Communication.Commands.CreateContactMessage
{
    public sealed record CreateContactMessageResponse(
        Guid Id,
        ContactStatus Status);
}

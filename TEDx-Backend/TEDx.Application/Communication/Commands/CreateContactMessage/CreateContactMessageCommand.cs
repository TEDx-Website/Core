using MediatR;
using TEDx.Domain.Common;

namespace TEDx.Application.Communication.Commands.CreateContactMessage
{
    public sealed record CreateContactMessageCommand(
        string Name,
        string Email,
        string Subject,
        string Message
    ) : IRequest<Result<CreateContactMessageResponse>>;
}

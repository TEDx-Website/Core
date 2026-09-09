using MediatR;
using TEDx.Domain.Common;

namespace TEDx.Application.Identity.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(
    Guid UserId,
    string Token)
    : IRequest<Result<ConfirmEmailResponse>>;

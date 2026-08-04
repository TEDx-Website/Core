using MediatR;
using TEDx.Domain.Common;

namespace TEDx.Application.Identity.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(
    string Email,
    string Token)
    : IRequest<Result<Unit>>;

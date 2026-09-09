using MediatR;
using TEDx.Domain.Common;

namespace TEDx.Application.Identity.Commands.ResetPassword;

public sealed record ResetPasswordCommand(
    string Email,
    string Token,
    string NewPassword,
    string ConfirmPassword)
    : IRequest<Result<Unit>>;

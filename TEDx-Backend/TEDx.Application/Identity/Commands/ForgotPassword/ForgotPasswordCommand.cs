using MediatR;
using TEDx.Domain.Common;

namespace TEDx.Application.Identity.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email)
    : IRequest<Result<Unit>>;

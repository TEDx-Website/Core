using MediatR;
using TEDx.Domain.Common;

namespace TEDx.Application.Identity.Commands.ResendConfirmation;

public sealed record ResendConfirmationCommand(string Email)
    : IRequest<Result<Unit>>;

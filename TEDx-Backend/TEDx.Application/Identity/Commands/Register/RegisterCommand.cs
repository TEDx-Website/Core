using MediatR;
using TEDx.Domain.Common;

namespace TEDx.Application.Identity.Commands.Register;

public sealed record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword)
    : IRequest<Result<RegisterResponse>>;

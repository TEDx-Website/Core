using MediatR;
using TEDx.Application.Identity.Common;
using TEDx.Domain.Common;

namespace TEDx.Application.Identity.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password)
    : IRequest<Result<AuthTokensResponse>>;

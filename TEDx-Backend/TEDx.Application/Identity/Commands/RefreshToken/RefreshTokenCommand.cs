using MediatR;
using TEDx.Application.Identity.Commands.Login;
using TEDx.Domain.Common;

namespace TEDx.Application.Identity.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthTokensResponse>>;

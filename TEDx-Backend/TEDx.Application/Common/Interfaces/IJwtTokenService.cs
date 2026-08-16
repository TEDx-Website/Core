using TEDx.Domain.Identity.Entities;

namespace TEDx.Application.Common.Interfaces;

public interface IJwtTokenService
{
    AccessTokenResult CreateAccessToken(User user);
}

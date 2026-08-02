using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Application.Identity.DTOs.Login
{
    public sealed class LoginResponse
    {
        public LoginResponse(string accessToken, int accessTokenExpiresIn, string refreshToken, int refreshTokenExpiresIn, UserSummary user)
        {
            AccessToken = accessToken;
            AccessTokenExpiresIn = accessTokenExpiresIn;
            RefreshToken = refreshToken;
            RefreshTokenExpiresIn = refreshTokenExpiresIn;
            User = user;
        }
        public string AccessToken { get; }
        public int AccessTokenExpiresIn { get; }      // 900
        public string RefreshToken { get; }
        public int RefreshTokenExpiresIn { get; }     // 604800
        public UserSummary User { get; }
    }
}

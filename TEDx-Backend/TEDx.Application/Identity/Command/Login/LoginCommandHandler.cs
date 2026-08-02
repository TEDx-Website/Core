using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TEDx.Application.Common.Interfaces;
using TEDx.Application.Identity.Command.Login;
using TEDx.Application.Identity.DTOs.Login;
using TEDx.Domain.Common;
using TEDx.Domain.Identity.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;
using TEDx.Application.Common.Errors;
namespace TEDx.Application.Identity.Command.Login
{
    internal sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtTokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokens;
        //private readonly ICurrentIpAddress _ip;
        private readonly IClock _clock;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(UserManager<User> userManager, IJwtTokenService tokenService, IRefreshTokenService refreshTokens, IClock clock, ILogger<LoginCommandHandler> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _refreshTokens = refreshTokens;
            _clock = clock;
            _logger = logger;
        }
        public async Task<Result<AuthResponse>> Handle(
            LoginCommand request, CancellationToken ct)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            // 1 — مش موجود
            if (user is null)
                return Result<AuthResponse>.Failure(Errors_Identity.InvalidCredentials);

            // 2 — مقفول
            if (await _userManager.IsLockedOutAsync(user))
            {
                _logger.LogWarning("Login attempt on locked account {AccountId}.", user.Id);
                return Result<AuthResponse>.Failure(Errors_Identity.InvalidCredentials);
            }

            // 3 — الباسورد
            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                await _userManager.AccessFailedAsync(user);      // ← العداد
                return Result<AuthResponse>.Failure(Errors_Identity.InvalidCredentials);
            }

            // 4 — موقوف (بعد الباسورد عن قصد)
            if (!user.IsActive)
            {
                _logger.LogWarning("Login on deactivated account {AccountId}.", user.Id);
                return Result<AuthResponse>.Failure(Errors_Identity.AccountDeactivated);
            }

            // 5 — مش مأكّد (بعد الإيقاف عن قصد برضه)
            if (!user.EmailConfirmed)
            {
                _logger.LogInformation("Login on unconfirmed account {AccountId}.", user.Id);
                return Result<AuthResponse>.Failure(Errors_Identity.InvalidCredentials);
            }

            await _userManager.ResetAccessFailedCountAsync(user);

            var access = _tokenService.CreateAccessToken(user);
            var refresh = await _refreshTokens.GenerateAndStoreAsync(user.Id, null, ct); //ICurrentIpAddress لحد ما نشوف ال 

            _logger.LogInformation("Successful login for {AccountId}.", user.Id);

            return Result<AuthResponse>.Success(new AuthResponse(
                access.Token,
                access.ExpiresInSeconds,
                refresh.RawToken,
                (int)(refresh.ExpiresAtUtc - _clock.UtcNow).TotalSeconds,
                new UserSummary(
                    user.Id,
                    user.Email!,
                    user.Role.ToString(),
                    user.FirstName!,
                    user.LastName!)));
        }
    }
}

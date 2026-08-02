using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TEDx.Application.Common.Interfaces;
using TEDx.Application.Common.Errors;

using TEDx.Application.Identity.DTOs.Register;
using TEDx.Domain.Common;
using TEDx.Domain.Identity.Entities;
using TEDx.Domain.Identity.Enums;
namespace TEDx.Application.Identity.Command.Register
{
    public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;
        //private readonly FrontendOptions _frontend;
        private readonly IClock _clock;
        private readonly ILogger<RegisterCommandHandler> _logger;

        public RegisterCommandHandler(
            UserManager<User> userManager,
            IEmailSender emailSender,
            //IOptions<FrontendOptions> frontend,
            IClock clock,
            ILogger<RegisterCommandHandler> logger)
        {
            _userManager = userManager;
            _emailSender = emailSender;
           // _frontend = frontend.Value;
            _clock = clock;
            _logger = logger;
        }

        public async Task<Result<RegisterResponse>> Handle(
            RegisterCommand request, CancellationToken ct)
        {
            var normalized = request.Email.ToUpperInvariant();

            var exists = await _userManager.Users
                .IgnoreQueryFilters()
                .AnyAsync(u => u.NormalizedEmail == normalized, ct);

            if (exists)
                return Result<RegisterResponse>.Failure(Errors_Identity.EmailTaken);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                UserName = request.Email,          
                LastName = request.LastName,
                Role = GlobalRole.Attendee,
                IsActive = true,
                EmailConfirmed = false,           
                CreatedAtUtc = _clock.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                _logger.LogWarning(
                    "User creation failed for {Email}: {Errors}",
                    request.Email,
                    string.Join("; ", result.Errors.Select(e => e.Code)));

                return Result<RegisterResponse>.Failure(Errors_Identity.WeakPassword);
            }

            _logger.LogInformation("New account registered: {AccountId}", user.Id);

            await SendConfirmationEmailAsync(user, ct);

            return Result<RegisterResponse>.Success(new RegisterResponse(
                user.Id, user.Email!, user.FirstName!, user.LastName!,
                user.Role, true));
        }

        private async Task SendConfirmationEmailAsync(User user, CancellationToken ct)
        {
            try
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                //var link = $"{_frontend.BaseUrl}/confirm-email" +
                //           $"?userId={Uri.EscapeDataString(user.Id.ToString())}" +
                //           $"&token={Uri.EscapeDataString(token)}";

               // await _emailSender.SendPasswordResetEmailAsync(user.Email!, link, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Confirmation email failed for {AccountId}. Account created; user can resend.",
                    user.Id);
            }
        }
    }
}

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TEDx.Api.Common.Respones;
using TEDx.Api.RateLimiting;
using TEDx.Application.Identity.Commands.ConfirmEmail;
using TEDx.Application.Identity.Commands.ForgotPassword;
using TEDx.Application.Identity.Commands.Login;
using TEDx.Application.Identity.Commands.Logout;
using TEDx.Application.Identity.Commands.RefreshToken;
using TEDx.Application.Identity.Commands.Register;
using TEDx.Application.Identity.Commands.ResendConfirmation;
using TEDx.Application.Identity.Commands.ResetPassword;
using TEDx.Application.Identity.Common;

namespace TEDx.Api.Controllers
{
    [Route("api/v1/auth")]
    public sealed class AuthController(ISender sender) : BaseApiController
    {
        [HttpPost("register")]
        [AllowAnonymous]
        [EnableRateLimiting(RateLimitPolicies.Auth)]
        [ProducesResponseType(typeof(ApiResponse<RegisterResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult> Register(
            [FromBody] RegisterCommand command,
            CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            return result.Match
                (
                onSuccess: data => Ok(ApiResponse<object>.SuccessResult(data)),
                onFailure: errors => Problem(errors)
                );
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [EnableRateLimiting(RateLimitPolicies.Auth)]
        [ProducesResponseType(typeof(ApiResponse<AuthTokensResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult> Login(
            [FromBody] LoginCommand command,
            CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            return result.Match
                (
                onSuccess: data => Ok(ApiResponse<object>.SuccessResult(data)),
                onFailure: errors => Problem(errors)
                );
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        [EnableRateLimiting(RateLimitPolicies.Auth)]
        [ProducesResponseType(typeof(ApiResponse<AuthTokensResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult> Refresh(
            [FromBody] RefreshTokenCommand command,
            CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            return result.Match
                (
                onSuccess: data => Ok(ApiResponse<object>.SuccessResult(data)),
                onFailure: errors => Problem(errors)
                );
        }

        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Logout(
            [FromBody] LogoutCommand? command,
            CancellationToken cancellationToken)
        {
            var result = await sender.Send(command ?? new LogoutCommand(null), cancellationToken);
            return result.Match
                (
                onSuccess: data => Ok(ApiResponse<object>.SuccessResult(data)),
                onFailure: errors => Problem(errors)
                );
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [EnableRateLimiting(RateLimitPolicies.AuthMail)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult> ForgotPassword(
            [FromBody] ForgotPasswordCommand command,
            CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            return result.Match
                (
                onSuccess: data => Ok(ApiResponse<object>.SuccessResult(data)),
                onFailure: errors => Problem(errors)
                );
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        [EnableRateLimiting(RateLimitPolicies.Auth)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult> ResetPassword(
            [FromBody] ResetPasswordCommand command,
            CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            return result.Match
                (
                onSuccess: data => Ok(ApiResponse<object>.SuccessResult(data)),
                onFailure: errors => Problem(errors)
                );
        }

        [HttpPost("confirm-email")]
        [AllowAnonymous]
        [EnableRateLimiting(RateLimitPolicies.Auth)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult> ConfirmEmail(
            [FromBody] ConfirmEmailCommand command,
            CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            return result.Match
                (
                onSuccess: data => Ok(ApiResponse<object>.SuccessResult(data)),
                onFailure: errors => Problem(errors)
                );
        }

        [HttpPost("resend-confirmation")]
        [AllowAnonymous]
        [EnableRateLimiting(RateLimitPolicies.AuthMail)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult> ResendConfirmation(
            [FromBody] ResendConfirmationCommand command,
            CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            return result.Match
                (
                onSuccess: data => Ok(ApiResponse<object>.SuccessResult(data)),
                onFailure: errors => Problem(errors)
                );
        }
    }
}

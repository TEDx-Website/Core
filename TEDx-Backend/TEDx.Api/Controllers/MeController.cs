using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TEDx.Api.Common.Respones;
using TEDx.Api.RateLimiting;
using TEDx.Application.Identity.Commands.ChangePassword;
using TEDx.Application.Identity.Queries.GetMyProfile;
using TEDx.Application.Identity.Commands.UpdateProfile;

namespace TEDx.Api.Controllers
{
    [Route("api/v1/me")]
    [Authorize]
    public sealed class MeController(ISender sender) : BaseApiController
    {
        [HttpPost("change-password")]
        [EnableRateLimiting(RateLimitPolicies.Auth)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult> ChangePassword(
            [FromBody] ChangePasswordCommand command,
            CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            return result.Match
                (
                onSuccess: data => Ok(ApiResponse<object>.SuccessResult(data)),
                onFailure: errors => Problem(errors)
                );
        }

        [HttpGet("My-Profile")]
        [EnableRateLimiting(RateLimitPolicies.Auth)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult> GetMyProfile( 
            CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetMyProfileQuery(), cancellationToken);
            return result.Match
                (
                onSuccess: data => Ok(ApiResponse<object>.SuccessResult(data)),
                onFailure: errors => Problem(errors)
                );
        }


        [HttpPut("Update-Profile")]
        [EnableRateLimiting(RateLimitPolicies.Auth)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult> UpdateProfile(
            [FromBody] UpdateMyProfileCommand command,
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

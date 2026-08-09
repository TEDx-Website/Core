using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TEDx.Api.Common.Respones;
using TEDx.Api.Filters;
using TEDx.Api.RateLimiting;
using TEDx.Application.Common.Errors;
using TEDx.Application.Identity.Commands.ChangePassword;
using TEDx.Application.Identity.Commands.UploadProfilePicture;
using TEDx.Application.Identity.Queries.GetMyProfile;
using TEDx.Application.Identity.Commands.UpdateProfile;

namespace TEDx.Api.Controllers
{
    [Route("api/v1/me")]
    [Authorize]
    public sealed class MeController(ISender sender) : BaseApiController
    {
        [HttpPost]
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

        [HttpGet]
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

        [HttpPut]
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

        [HttpPost("profile-picture")]
        [Consumes("multipart/form-data")]
        [EnableRateLimiting(RateLimitPolicies.Upload)]
        [TypeFilter(typeof(UploadSizeLimitFilter))]
        [ProducesResponseType(typeof(ApiResponse<ProfilePictureResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UploadProfilePicture(
            IFormFile file,
            CancellationToken cancellationToken)
        {
            if (file is null || file.Length == 0)
                return Problem([Errors_Media.FileMissing]);

            await using var content = file.OpenReadStream();

            var command = new UploadProfilePictureCommand(content, file.FileName);

            var result = await sender.Send(command, cancellationToken);

            return HandleResult(result, OkEnvelope);
        }
    }
}

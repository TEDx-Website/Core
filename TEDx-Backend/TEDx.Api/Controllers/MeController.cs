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
            return HandleNullData(result);
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

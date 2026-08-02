using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TEDx.Api.Common.Respones;
using TEDx.Application.Identity.Commands.Logout;
using TEDx.Application.Identity.Commands.RefreshToken;
using TEDx.Application.Identity.Common;

namespace TEDx.Api.Controllers
{
    [Route("api/v1/auth")]
    public sealed class AuthController(ISender sender) : BaseApiController
    {
        [HttpPost("refresh")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AuthTokensResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Refresh(
            [FromBody] RefreshTokenCommand command,
            CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            return HandleResult(result, OkEnvelope);
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
            return HandleNoContent(result);
        }
    }
}

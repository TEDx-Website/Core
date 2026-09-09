using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TEDx.Api.Common.Responses;
using TEDx.Api.RateLimiting;
using TEDx.Application.Communication.Commands.CreateContactMessage;

namespace TEDx.Api.Controllers
{
    [Route("api/v1/contact")]
    [AllowAnonymous]
    public sealed class ContactController(ISender sender) : BaseApiController
    {
        [HttpPost]
        [EnableRateLimiting(RateLimitPolicies.Contact)]
        [ProducesResponseType(typeof(ApiResponse<CreateContactMessageResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult> SubmitContactMessage(
            [FromBody] CreateContactMessageCommand command,
            CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);

            return HandleResult(result, CreatedEnvelope);
        }
    }
}

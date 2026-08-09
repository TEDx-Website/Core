using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TEDx.Api.Common.Respones;
using TEDx.Api.RateLimiting;
using TEDx.Application.Identity.Commands.ChangePassword;
using TEDx.Application.Ticketing.Command.CreateEvents;

namespace TEDx.Api.Controllers
{
    [Route("api/v1/admin/events")]
    [ApiController]
    public class EventController(ISender sender) : BaseApiController
    {

        [Authorize("Admin")]
        [HttpPost]
        [EnableRateLimiting(RateLimitPolicies.Auth)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult> CreateEvent(
           [FromBody] CreateEventCommand command,
           CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);
            return result.Match
                (
                onSuccess: data => CreatedEnvelope(data),
                onFailure: errors => Problem(errors)
                );
        }
    }
}

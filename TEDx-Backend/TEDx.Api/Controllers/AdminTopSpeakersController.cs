using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TEDx.Api.Common.Responses;
using TEDx.Application.Ticketing.Commands.AddToTopSpeakers;
using TEDx.Application.Ticketing.Commands.RemoveTopSpeaker;
using TEDx.Application.Ticketing.Commands.ReorderTopSpeakers;

namespace TEDx.Api.Controllers
{
    [Route("api/v1/admin/top-speakers")]
    [Authorize]
    public class AdminTopSpeakersController(ISender sender) : BaseApiController
    {
        [HttpPost("{speakerId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> AddTopSpeaker(
            Guid speakerId,
            CancellationToken cancellationToken)
        {
            var command = new AddTopSpeakerCommand(speakerId);

            var result = await sender.Send(command, cancellationToken);

            return HandleNoContent(result);
        }

        [HttpPut("reorder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ReorderTopSpeakers(
            [FromBody] ReorderTopSpeakersRequest request,
            CancellationToken cancellationToken)
        {
            var command = new ReorderTopSpeakersCommand(request.Items);

            var result = await sender.Send(command, cancellationToken);

            return HandleNoContent(result);
        }

        [HttpDelete("{speakerId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> RemoveTopSpeaker(
            Guid speakerId,
            CancellationToken cancellationToken)
        {
            var command = new RemoveTopSpeakerCommand(speakerId);

            var result = await sender.Send(command, cancellationToken);

            return HandleNoContent(result);
        }
    }
}

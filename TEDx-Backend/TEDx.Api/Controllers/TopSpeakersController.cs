using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TEDx.Api.Common.Responses;
using TEDx.Application.Ticketing.Commands.AddToTopSpeakers;
using TEDx.Application.Ticketing.Commands.RemoveTopSpeaker;
using TEDx.Application.Ticketing.Queries.GetTopSpeakers;

namespace TEDx.Api.Controllers
{
    [Route("api/v1/top-speakers")]
    [Authorize]
    public class TopSpeakersController(ISender sender) : BaseApiController
    {

        //Get
        [HttpGet("top")]
        [ProducesResponseType(typeof(ApiResponse<List<TopSpeakerResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetTopSpeakers(
        CancellationToken cancellationToken)
        {
            var query = new GetTopSpeakersQuery();

            var result = await sender.Send(query, cancellationToken);

            return HandleResult(result, OkEnvelope);
        }

        [HttpPost("{speakerId:guid}/top5")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> AddTopSpeaker(
        Guid speakerId,
        CancellationToken cancellationToken)
        {
            var command = new AddTopSpeakerCommand(speakerId);

            var result = await sender.Send(command, cancellationToken);

            return HandleResult(result, OkEnvelope);
        }
        [HttpDelete("{speakerId:guid}/top")]
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

            return HandleResult(result, OkEnvelope);
        }

    }
}

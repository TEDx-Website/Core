using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TEDx.Api.Common.Responses;
using TEDx.Application.Ticketing.Commands.CreateSpeaker;
using TEDx.Application.Ticketing.Commands.DeleteSpeaker;
using TEDx.Application.Ticketing.Commands.UpdateSpeaker;
using TEDx.Application.Ticketing.Queries.GetAdminSpeakers;

namespace TEDx.Api.Controllers
{
    [Route("api/v1/admin/speakers")]
    [Authorize]
    public class AdminSpeakersController : BaseApiController
    {
        private readonly ISender sender;
        public AdminSpeakersController(ISender sender)
        {
            this.sender = sender;
        }

        [HttpGet]
        public async Task<ActionResult> GetSpeakers(
        CancellationToken cancellationToken,
        [FromQuery] int? page = null,
        [FromQuery] int? pageSize = null)
        {
            var query = new GetAdminSpeakersQuery(
                page,
                pageSize);

            var result = await sender.Send(query, cancellationToken);

            return HandlePagedResult(result);
        }

        [HttpPut("{speakerId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult> UpdateSpeaker(
            Guid speakerId,
            [FromBody] UpdateSpeakerCommand command,
            CancellationToken cancellationToken)
        {
            var result = await sender.Send(
                command with { SpeakerId = speakerId },
                cancellationToken);

            return HandleNoContent(result);
        }
        // Post
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult> CreateSpeaker(
        [FromBody] CreateSpeakerCommand command,
        CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);

            return HandleNoContent(result);
        }
        // Delete
        [HttpDelete("{speakerId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> DeleteSpeaker(
    Guid speakerId,
    CancellationToken cancellationToken)
        {
            var command = new DeleteSpeakerCommand(speakerId);

            var result = await sender.Send(command, cancellationToken);

            return HandleNoContent(result);
        }



    }
}

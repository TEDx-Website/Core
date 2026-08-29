using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TEDx.Api.Common.Responses;
using TEDx.Application.Communication.Dtos;
using TEDx.Application.Communication.Queries.GetContactSubmissions;
using TEDx.Application.Ticketing.Commands.UpdateContactStatus;
namespace TEDx.Api.Controllers
{
    [Route("api/v1/admin")]
    [ApiController]
    [Authorize]
    public class AdminContactController : BaseApiController
    {
        private readonly ISender sender;
        public AdminContactController(ISender sender)
        {
            this.sender = sender;
        }

        [HttpGet("contact-submissions")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ContactSubmissionListItemDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> GetContactSubmissions(
            CancellationToken ct,
            [FromQuery] int? page = null,
            [FromQuery] int? pageSize = null,
            [FromQuery] string? status = null,
            [FromQuery] string? sort = null)
        {
            var query = new GetContactSubmissionsQuery(page, pageSize, status, sort);
            var result = await sender.Send(query, ct);
            return HandlePagedResult(result);
        }

        [HttpPatch("contact-submissions/{id:guid}/status")]
        [ProducesResponseType(typeof(ApiResponse<UpdateContactStatusResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateContactStatusById(
            [FromRoute] Guid id,
            [FromBody] UpdateContactStatusCommand command,
            CancellationToken ct)
        {
            var request = command with { Id = id };
            var result = await sender.Send(request, ct);
            return HandleResult(result, OkEnvelope);
        }
    }
}

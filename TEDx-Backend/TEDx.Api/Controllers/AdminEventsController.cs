using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TEDx.Api.Common.Respones;
using TEDx.Application.Ticketing.DTOs;
using TEDx.Application.Ticketing.Queries.GetAdminEvents;

namespace TEDx.Api.Controllers
{
    [Route("api/v1/admin/events")]
    [Authorize]
    public sealed class AdminEventsController(ISender sender) : BaseApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AdminEventListItemDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult> GetEvents(
            CancellationToken cancellationToken,
            [FromQuery] int? page = null,
            [FromQuery] int? pageSize = null,
            [FromQuery] string? sort = null,
            [FromQuery] string? status = null,
            [FromQuery] string? search = null)
        {
            var query = new GetAdminEventsQuery(page, pageSize, sort, status, search);

            var result = await sender.Send(query, cancellationToken);

            return HandlePagedResult(result);
        }
    }
}

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TEDx.Api.Common.Respones;
using TEDx.Api.RateLimiting;
using TEDx.Application.Ticketing.Command.CreateEvents;
using TEDx.Application.Ticketing.Command.DeleteEvent;
using TEDx.Application.Ticketing.DTOs;
using TEDx.Application.Ticketing.Queries.GetAdminEvents;
using TEDx.Application.Ticketing.Queries.GetEventOrders;
using TEDx.Domain.Ticketing.Enums;

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
        [Authorize]
        [HttpPost]
        [EnableRateLimiting(RateLimitPolicies.Auth)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
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
        [Authorize]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> DeleteEvent(
        Guid id,
        CancellationToken cancellationToken)
        {
            var result = await sender.Send(
                new DeleteEventCommand { EventId = id },
                cancellationToken);

            return result.Match(
                onSuccess: data => Ok(ApiResponse<object>.SuccessResult(data)),
                onFailure: errors => Problem(errors)
            );
        }
        [Authorize]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> GetEventOrders(
          Guid id, [FromQuery] int page, [FromQuery] int pageSize, [FromQuery] OrderStatus? status
           ,CancellationToken cancellationToken)
        {
            // Use the positional constructor
            var query = new GetEventOrdersQuery(id, page, pageSize, status);
            var result = await sender.Send(query, cancellationToken);

            return result.Match(
                onSuccess: data => Ok(ApiResponse<object>.SuccessResult(data)),
                onFailure: errors => Problem(errors)
            );
        }
    }
}

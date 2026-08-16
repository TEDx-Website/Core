using System;
using System.Threading;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TEDx.Api.Common.Respones;
using TEDx.Api.RateLimiting;
using TEDx.Api.Requests.Events;
using TEDx.Application.Common.Errors;
using TEDx.Application.Ticketing.Command.CancelEvent;
using TEDx.Application.Ticketing.Command.CreateEvents;
using TEDx.Application.Ticketing.Command.DeleteEvent;
using TEDx.Application.Ticketing.Command.UpdateEvent;
using TEDx.Application.Ticketing.Command.ChangeEventStatus;
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

        [HttpPost]
        [EnableRateLimiting(RateLimitPolicies.Auth)]
        [ProducesResponseType(typeof(ApiResponse<CreateEventDTO>), StatusCodes.Status201Created)]
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

            return HandleResult(result, CreatedEnvelope);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> DeleteEvent(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
        {
            var result = await sender.Send(
                new DeleteEventCommand { EventId = id },
                cancellationToken);

            return HandleNullData(result);
        }

        [HttpGet("{id:guid}/orders")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<EventOrderDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult> GetEventOrders(
            [FromRoute] Guid id,
            CancellationToken cancellationToken,
            [FromQuery] int? page = null,
            [FromQuery] int? pageSize = null,
            [FromQuery] OrderStatus? status = null)
        {
            var query = new GetEventOrdersQuery(id, page, pageSize, status);

            var result = await sender.Send(query, cancellationToken);

            return HandlePagedResult(result);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<UpdateEventDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult> UpdateEvent(
            [FromRoute] Guid id,
            [FromBody] UpdateEventRequest request,
            CancellationToken cancellationToken)
        {
            if (!TryDecodeRowVersion(request.RowVersion, out var rowVersion))
                return Problem(new[] { Errors_Common.InvalidRowVersion });


            var command = new UpdateEventCommand(
                EventId: id,
                TitleEn: request.TitleEn,
                TitleAr: request.TitleAr,
                DescriptionEn: request.DescriptionEn,
                DescriptionAr: request.DescriptionAr,
                StartsAtUtc: request.StartsAtUtc,
                EndsAtUtc: request.EndsAtUtc,
                Location: request.Location,
                Capacity: request.Capacity,
                TicketPrice: request.TicketPrice,
                MaxIndividualQtyPerOrder: request.MaxIndividualQtyPerOrder,
                RowVersion: rowVersion
            );

            var result = await sender.Send(command, cancellationToken);

            return HandleResult(result, OkEnvelope);
        }

        [HttpPost("{id:guid}/status")]
        [ProducesResponseType(typeof(ApiResponse<ChangeEventStatusDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult> ChangeEventStatus(
        [FromRoute] Guid id,
        [FromBody] ChangeEventStatusRequest request,
        CancellationToken cancellationToken)
        {
            var command = new ChangeEventStatusCommand(
                id,
                request.Status
            );

            var result = await sender.Send(command, cancellationToken);

            return HandleResult(result, OkEnvelope);
        }

        [HttpPost("{id:guid}/cancel")]
        [ProducesResponseType(typeof(ApiResponse<CancelEventResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> CancelEvent(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var command = new CancelEventCommand(id);

            var result = await sender.Send(command, cancellationToken);

            return HandleResult(result, OkEnvelope);
        }
    }
}

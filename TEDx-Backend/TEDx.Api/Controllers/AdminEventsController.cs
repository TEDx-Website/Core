using System;
using System.Threading;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TEDx.Api.Common.Responses;
using TEDx.Api.RateLimiting;
using TEDx.Api.Requests.Events;
using TEDx.Application.Common.Errors;
using TEDx.Application.Ticketing.Commands.CancelEvent;
using TEDx.Application.Ticketing.Commands.CreateEvent;
using TEDx.Application.Ticketing.Commands.DeleteEvent;
using TEDx.Application.Ticketing.Commands.UpdateEvent;
using TEDx.Application.Ticketing.Commands.ChangeEventStatus;
using TEDx.Application.Ticketing.Dtos;
using TEDx.Application.Ticketing.Queries.GetAdminEvents;
using TEDx.Application.Ticketing.Queries.GetEventOrders;
using TEDx.Application.Ticketing.Queries.GetEventPromoCodes;
using TEDx.Domain.Ticketing.Enums;
namespace TEDx.Api.Controllers
{

    [Route("api/v1/admin/events")]
    [Authorize]
    public sealed class AdminEventsController(ISender sender) : BaseApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AdminEventListItemDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
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
        [ProducesResponseType(typeof(ApiResponse<CreateEventResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult> CreateEvent(
           [FromBody] CreateEventCommand command,
           CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);

            return HandleResult(result, CreatedEnvelope);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> DeleteEvent(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
        {
            var result = await sender.Send(
                new DeleteEventCommand(id),
                cancellationToken);

            return HandleNullData(result);
        }

        [HttpGet("{id:guid}/orders")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<EventOrderDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
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

        [HttpGet("{eventId:guid}/promo-codes")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<EventPromoCodeDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult> GetEventPromoCodes(
            [FromRoute] Guid eventId,
            CancellationToken cancellationToken,
            [FromQuery] int? page = null,
            [FromQuery] int? pageSize = null)
        {
            var query = new GetEventPromoCodesQuery(eventId, page, pageSize);

            var result = await sender.Send(query, cancellationToken);

            return HandlePagedResult(result);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<UpdateEventResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult> UpdateEvent(
            [FromRoute] Guid id,
            [FromBody] UpdateEventRequest request,
            CancellationToken cancellationToken)
        {
            if (!TryDecodeRowVersion(request.RowVersion, out var rowVersion))
                return Problem(new[] { CommonErrors.InvalidRowVersion });


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
        [ProducesResponseType(typeof(ApiResponse<ChangeEventStatusResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
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
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
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

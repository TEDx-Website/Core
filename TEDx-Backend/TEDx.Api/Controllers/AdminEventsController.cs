using System;
using System.Threading;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TEDx.Api.Common.Respones;
using TEDx.Api.RateLimiting;
using TEDx.Application.Common.Errors;
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
        
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetEventOrders(
          [FromRoute] Guid id, [FromQuery] int page, [FromQuery] int pageSize, [FromQuery] OrderStatus? status
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

        [HttpPut("{eventId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<UpdateEventDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult> UpdateEvent(
            [FromRoute] Guid eventId,
            [FromBody] UpdateEventCommand request,
            CancellationToken cancellationToken)
        {
            if (!TryDecodeRowVersion(request.RowVersion, out var rowVersion))
                return Problem(new[] { Errors_Common.InvalidRowVersion });

            var command = new UpdateEventCommand(
                EventId: eventId,
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

            return HandleResult(result, data => OkEnvelope(data));
        }

        [HttpPut("{eventId:guid}/status")]
        [ProducesResponseType(typeof(ApiResponse<ChangeEventStatusDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult> ChangeEventStatus(
        [FromRoute] Guid eventId,
        [FromBody] ChangeEventStatusCommand request,
        CancellationToken cancellationToken)
        {

            var command = new ChangeEventStatusCommand(
                request.Id,
                request.TargetStatus
            );
            var result = await sender.Send(command, cancellationToken);


            return HandleResult(result, data => OkEnvelope(data));
        }
        private static bool TryDecodeRowVersion(string? value, out byte[] rowVersion)
        {
            rowVersion = [];

            if (string.IsNullOrWhiteSpace(value))
                return false;

            var buffer = new byte[((value.Length * 3) + 3) / 4];

            if (!Convert.TryFromBase64String(value, buffer, out var bytesWritten) || bytesWritten == 0)
                return false;

            rowVersion = buffer[..bytesWritten];
            return true;
        }
    }
}

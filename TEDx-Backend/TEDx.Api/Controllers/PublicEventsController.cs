using CloudinaryDotNet;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TEDx.Api.Common.Responses;
using TEDx.Application.Ticketing.Dtos;
using TEDx.Application.Ticketing.Queries.GetAdminEvents;
using TEDx.Application.Ticketing.Queries.GetPublicEvents;
using TEDx.Domain.Ticketing.Entities;

namespace TEDx.Api.Controllers
{
    [Route("api/v1/events")]
    [ApiController]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public sealed class PublicEventsController(ISender sender) : BaseApiController
    {
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<GetPublicEventResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult> GetPublicEvents([FromQuery] GetPublicEventsQuery query,
            CancellationToken cancellationToken)
        {
            var result = await sender.Send(query, cancellationToken);

            return HandlePagedResult(result);
        }
    }
}

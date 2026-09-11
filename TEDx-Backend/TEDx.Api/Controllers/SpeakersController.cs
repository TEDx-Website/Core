using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TEDx.Api.Common.Responses;
using TEDx.Application.Ticketing.Queries.GetNextSpeaker;
using TEDx.Application.Ticketing.Queries.GetSpeakers;

namespace TEDx.Api.Controllers
{
    [Route("api/v1/speakers")]
    [AllowAnonymous]
    public class SpeakersController(ISender sender) : BaseApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<SpeakerCatalogResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetSpeakers(
            [FromQuery] string? search,
            [FromQuery] string? category,
            CancellationToken cancellationToken)
        {
            var query = new GetSpeakersQuery(search, category);
            var result = await sender.Send(query, cancellationToken);
            return HandleResult(result, OkEnvelope);
        }

        [HttpGet("next")]
        [ProducesResponseType(typeof(ApiResponse<NextSpeakerResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetNextSpeaker(
            CancellationToken cancellationToken)
        {
            var query = new GetNextSpeakerQuery();
            var result = await sender.Send(query, cancellationToken);
            return HandleResult(result, OkEnvelope);
        }
    }
}

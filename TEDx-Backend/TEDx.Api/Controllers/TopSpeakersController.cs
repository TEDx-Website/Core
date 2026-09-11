using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TEDx.Api.Common.Responses;
using TEDx.Application.Ticketing.Queries.GetTopSpeakers;

namespace TEDx.Api.Controllers
{
    [Route("api/v1/top-speakers")]
    [AllowAnonymous]
    public class TopSpeakersController(ISender sender) : BaseApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<TopSpeakerResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetTopSpeakers(
            [FromQuery] int? limit,
            CancellationToken cancellationToken)
        {
            var query = new GetTopSpeakersQuery(limit);

            var result = await sender.Send(query, cancellationToken);

            return HandleResult(result, OkEnvelope);
        }
    }
}

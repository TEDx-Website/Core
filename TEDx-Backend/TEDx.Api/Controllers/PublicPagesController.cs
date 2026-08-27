using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TEDx.Api.Common.Responses;
using TEDx.Application.Ticketing.Commands.UpdateContactStatus;
namespace TEDx.Api.Controllers
{
    [Route("api/v1/admin")]
    [ApiController]
    [Authorize]
    public class PublicPagesController : BaseApiController
    {
        private readonly ISender sender;
        public PublicPagesController(ISender sender)
        {
            this.sender = sender;
        }
        [HttpPut("contact-submissions/{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<UpdateContactStatusResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateContactstatusById(
            [FromRoute] Guid id,
            [FromBody] UpdateContactStatusCommand command,
            CancellationToken ct)
        {
            var request = command with { Id = id };
            var result = await sender.Send(request, ct);
            return HandleResult(result,OkEnvelope);
        }   
    }
}

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TEDx.Application.Identity.Command.Login;
using TEDx.Application.Identity.Command.Register;
using TEDx.Application.Identity.DTOs.Login;

namespace TEDx.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator) => _mediator = mediator;

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Register(
            RegisterCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            return result.Match<IActionResult>(
                value => Created($"/api/v1/users/{value.Id}", value),
                errors => UnprocessableEntity(errors)
            );
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Login(
            LoginCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return HandleResult(result, OkEnvelope);
        }

    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TEDx.Api.Common.Respones;
using TEDx.Api.Mapping;
using TEDx.Domain.Common;

namespace TEDx.Api.Controllers
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected ActionResult HandleResult<T>(
            Result<T> result,
            Func<T, ActionResult> onSuccess)
        {
            return result.Match(
                onSuccess,
                Problem);
        }

        protected ActionResult HandleNoContent<T>(Result<T> result)
        {
            return result.Match(
                _ => NoContentEnvelope(),
                Problem);
        }

        protected ActionResult Problem(IReadOnlyList<Error> errors)
        {
            var mapped = ErrorResultMapper.Map(errors, GetTraceId());
            return StatusCode(mapped.StatusCode, mapped.Body);
        }

        protected ActionResult OkEnvelope<T>(T data)
        {
            return Ok(ApiResponse<T>.SuccessResult(data));
        }

        protected ActionResult CreatedEnvelope<T>(T data)
        {
            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<T>.SuccessResult(data));
        }

        protected ActionResult NoContentEnvelope()
        {
            return NoContent();
        }

        private string? GetTraceId()
            => HttpContext.Items["CorrelationId"] as string;
    }
}

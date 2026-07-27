using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using TEDx.Api.Common.Respones;
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

        protected ActionResult Problem(IReadOnlyList<Error> errors)
        {
            if (errors is null || errors.Count == 0)
            {
                var nullresponse = ApiResponse<object>.FailureResult(
                new ApiErrorResponse
                {
                    Code = "INTERNAL_SERVER_ERROR",
                    Description = "An unexpected error occurred."
                });

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    nullresponse);
            }

            var firstError = errors[0];

            var apiError = new ApiErrorResponse
            {
                Code = firstError.Code,
                Description = firstError.Description,

                FieldErrors = firstError.Type == ErrorType.Validation
                    ? errors
                        .GroupBy(e => e.Code)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.Description).ToArray())
                    : null
            };

            var response = ApiResponse<object>.FailureResult(apiError);

            return firstError.Type switch
            {
                ErrorType.Validation =>
                    UnprocessableEntity(response), // 422

                ErrorType.NotFound =>
                    NotFound(response), // 404

                ErrorType.Conflict =>
                    Conflict(response), // 409

                ErrorType.Unauthorized =>
                    Unauthorized(response), // 401

                ErrorType.Forbidden =>
                    StatusCode(StatusCodes.Status403Forbidden, response), // 403

                _ =>
                    StatusCode(StatusCodes.Status500InternalServerError, response)
            };
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
     }

}

using TEDx.Api.Common.Responses;

namespace TEDx.Api.Middleware
{
    public sealed class GlobalExceptionMiddleware
    {
        private const string CorrelationItemKey = "CorrelationId";

        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                var correlationId = context.Items[CorrelationItemKey] as string
                    ?? context.TraceIdentifier;

                _logger.LogError(
                    exception,
                    "Unhandled exception occurred. CorrelationId: {CorrelationId}",
                    correlationId);

                await HandleExceptionAsync(context, correlationId);
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            string correlationId)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = ApiResponse<object>.FailureResult(new ApiError
            {
                Code = "INTERNAL_SERVER_ERROR",
                Message = "An unexpected error occurred.",
                TraceId = correlationId,
            });

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}

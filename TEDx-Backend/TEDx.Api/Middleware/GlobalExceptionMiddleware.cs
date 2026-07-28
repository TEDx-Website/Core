using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TEDx.Api.Common.Respones;
namespace TEDx.Api.Middleware
{
    public sealed class GlobalExceptionMiddleware 
    {
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
                var correlationId = context.TraceIdentifier;

                _logger.LogError(
                    exception,
                    "Unhandled exception occurred. CorrelationId: {CorrelationId}",
                    correlationId);


                await HandleExceptionAsync(
                    context,
                    correlationId);
            }
        }


        private static async Task HandleExceptionAsync(
            HttpContext context,
            string correlationId)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode =
                StatusCodes.Status500InternalServerError;


            var response = new
            {
                statusCode = context.Response.StatusCode,
                message = "An unexpected error occurred.",
                correlationId
            };


            await context.Response.WriteAsJsonAsync(response);
        }

    }
}

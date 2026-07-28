using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TEDx.Api.Common.Respones;
using Microsoft.Extensions.Logging;
namespace TEDx.Api.Middleware
{
    public sealed class GlobalExceptionMiddleware 
    {
        private readonly RequestDelegate _next; // بيمثلل اللي بعدك ف Pipeline
                                                // 
        private readonly ILogger<GlobalExceptionMiddleware> _logger; // Serilog

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
                await HandleExceptionAsync(
                    context,
                    exception
                    );
            }
        }
        private async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            _logger.LogError(
                exception,
                "Unhandled exception occurred.");

            context.Response.ContentType = "application/json";

            context.Response.StatusCode =
                StatusCodes.Status500InternalServerError;

            var correlationId = context.Items["CorrelationId"]?.ToString() ?? context.TraceIdentifier;

            var response = new ErrorResponse
            (
                "An unexpected error occurred.",
                correlationId
            
            );
            await context.Response.WriteAsJsonAsync(response);
        }

    }
}

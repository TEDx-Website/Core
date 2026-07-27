using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace TEDx.Application.Common.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        using (logger.BeginScope("{RequestName}", requestName))
        {
            logger.LogInformation("Handling {RequestName}", requestName);

            try
            {
                var response = await next();
                stopwatch.Stop();
                logger.LogInformation(
                    "Handled {RequestName} in {ElapsedMilliseconds}ms",
                    requestName, stopwatch.ElapsedMilliseconds);
                return response;
            }
            catch (Exception)
            {
                stopwatch.Stop();
                logger.LogError(
                    "Unhandled exception in {RequestName} after {ElapsedMilliseconds}ms",
                    requestName, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}

using Serilog.Context;

namespace TEDx.Api.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string HeaderName = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext context)
    {
        //var Incoming = context.Request.Headers[HeaderName].FirstOrDefault();

        //var correlationId = Guid.TryParse(Incoming, out var parsed)
        //    ? parsed.ToString("N")
        //    : Guid.NewGuid().ToString("N");
        var correlationId = context.Request.Headers[HeaderName].FirstOrDefault()
           ?? Guid.NewGuid().ToString("N");

        context.Response.Headers[HeaderName] = correlationId;
        context.Items["CorrelationId"] = correlationId;

        using (LogContext.PushProperty("CorrelationId", correlationId))
            await next(context);
    }
}

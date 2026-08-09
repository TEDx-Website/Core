using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using TEDx.Api.Mapping;
using TEDx.Application.Common.Errors;
using TEDx.Infrastructure.Configuration;

namespace TEDx.Api.Filters;

public sealed class UploadSizeLimitFilter(
    IOptions<CloudinaryOptions> options,
    ILogger<UploadSizeLimitFilter> logger) : IAsyncResourceFilter
{
    private const long MultipartOverheadBytes = 8 * 1024;

    private readonly long _maxFileSizeBytes = options.Value.MaxFileSizeBytes;

    public async Task OnResourceExecutionAsync(
        ResourceExecutingContext context,
        ResourceExecutionDelegate next)
    {
        ApplyBodySizeLimit(context.HttpContext);

        var executed = await next();

        if (executed.ExceptionHandled ||
            executed.Exception is not BadHttpRequestException failure ||
            failure.StatusCode != StatusCodes.Status413PayloadTooLarge)
        {
            return;
        }

        var correlationId = context.HttpContext.Items["CorrelationId"] as string;

        logger.LogInformation(
            "Rejected an upload exceeding the configured {MaxFileSizeBytes} byte ceiling. CorrelationId: {CorrelationId}",
            _maxFileSizeBytes,
            correlationId);

        var mapped = ErrorResultMapper.Map([Errors_Media.FileTooLarge], correlationId);

        executed.ExceptionHandled = true;
        executed.Result = new ObjectResult(mapped.Body) { StatusCode = mapped.StatusCode };
    }

    private void ApplyBodySizeLimit(HttpContext httpContext)
    {
        var feature = httpContext.Features.Get<IHttpMaxRequestBodySizeFeature>();

        if (feature is null || feature.IsReadOnly)
            return;

        feature.MaxRequestBodySize = _maxFileSizeBytes + MultipartOverheadBytes;
    }
}

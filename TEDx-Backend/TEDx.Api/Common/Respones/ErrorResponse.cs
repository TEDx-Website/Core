namespace TEDx.Api.Common.Respones
{
    public sealed record ErrorResponse
    (
             string Message,
             string CorrelationId
    );
}

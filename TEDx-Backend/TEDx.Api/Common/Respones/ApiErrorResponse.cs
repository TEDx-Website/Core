namespace TEDx.Api.Common.Respones
{
    public sealed class ApiErrorResponse
    {
        public string Code { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;

        public Dictionary<string, string[]>? FieldErrors { get; init; }
    }
}

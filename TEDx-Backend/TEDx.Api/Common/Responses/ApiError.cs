namespace TEDx.Api.Common.Responses
{
    public sealed class ApiError
    {
        /// Not Status Code Like (404, 200, ...)
        /// It's Stable machine-readable error code (Ex: `VALIDATION_ERROR` / `USER_NOT_FOUND`)
        public string Code { get; init; } = string.Empty;

        /// Human-readable message. (the SPA reads) (Ex: "The email address is already registered.")
        public string Message { get; init; } = string.Empty;

        /// Per-field validation messages, keyed by field name. Present only for validation failures.
        /// {
        ///  "email": ["Invalid email format."],
        ///  "password": ["Password is required.", "Too short."]
        /// }
        public Dictionary<string, string[]>? FieldErrors { get; init; }

        /// Correlation id so a client-facing error can be matched to the server logs
        public string? TraceId { get; init; }
    }
}

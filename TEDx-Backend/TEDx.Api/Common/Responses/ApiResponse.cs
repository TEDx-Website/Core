using System.Text.Json.Serialization;

namespace TEDx.Api.Common.Responses
{
    public sealed class ApiResponse<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
        public ApiError? Error { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PagedMeta? Meta { get; init; }

        public static ApiResponse<T> SuccessResult(T? data)
        => new()
        {
            Success = true,
            Data = data,
            Error = null
        };

        public static ApiResponse<T> SuccessResult(T? data, PagedMeta meta)
        => new()
        {
            Success = true,
            Data = data,
            Error = null,
            Meta = meta
        };

        public static ApiResponse<T> FailureResult(ApiError error)
        => new()
        {
            Success = false,
            Data = default,
            Error = error
        };
    }
}

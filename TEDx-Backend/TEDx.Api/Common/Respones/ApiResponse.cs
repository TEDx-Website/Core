namespace TEDx.Api.Common.Respones
{
    public sealed class ApiResponse<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
        public ApiErrorResponse? Error { get; init; }
        public static ApiResponse<T> SuccessResult(T? data)
        => new()
        {
            Success = true,
            Data = data,
            Error = null
        };

        public static ApiResponse<T> FailureResult(ApiErrorResponse error)
        => new()
        {
            Success = false,
            Data = default,
            Error = error
        };
    }
}

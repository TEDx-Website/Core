namespace TEDx.Api.Common.Responses
{
    public sealed class PagedMeta
    {
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int TotalItems { get; init; }
        public int TotalPages { get; init; }
    }
}

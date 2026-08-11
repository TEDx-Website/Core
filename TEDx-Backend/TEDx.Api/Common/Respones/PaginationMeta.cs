namespace TEDx.Api.Common.Respones
{
    public sealed class PaginationMeta
    {
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int TotalItems { get; init; }
        public int TotalPages { get; init; }
    }
}

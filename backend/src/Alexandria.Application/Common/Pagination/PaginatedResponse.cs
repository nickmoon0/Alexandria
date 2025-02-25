namespace Alexandria.Application.Common.Pagination;

public class PagingData
{
    public Guid? NextCursor { get; set; }
}

public class PaginatedResponse<TEntity>
{
    public required IReadOnlyList<TEntity> Data { get; set; }
    public required PagingData Paging { get; set; }
}
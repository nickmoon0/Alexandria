namespace Alexandria.Application.Common.Pagination;

public class PaginatedRequest
{
    public Guid? CursorId { get; set; }
    public int PageSize { get; set; } = 25;
}
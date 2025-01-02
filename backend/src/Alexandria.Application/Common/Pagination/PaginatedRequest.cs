namespace Alexandria.Application.Common.Pagination;

public enum PaginationDirection
{
    NextPage,
    PreviousPage
}

public class PaginatedRequest
{
    public Guid? CursorId { get; set; }
    public int PageSize { get; set; } = 10;
    
    public PaginationDirection? Direction { get; set; } = PaginationDirection.NextPage;
}
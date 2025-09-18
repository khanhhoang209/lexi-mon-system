namespace LexiMon.Service.ApiResponse;

public class PaginatedResponse<T> : ServiceResponse
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    public T? Data { get; set; } = default!;
}
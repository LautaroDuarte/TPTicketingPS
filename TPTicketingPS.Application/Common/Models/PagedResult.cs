namespace TPTicketingPS.Application.Common.Models;

public sealed record PagedResult<T>(
    IReadOnlyCollection<T> Data,
    int Page,
    int PageSize,
    int TotalItems)
{
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
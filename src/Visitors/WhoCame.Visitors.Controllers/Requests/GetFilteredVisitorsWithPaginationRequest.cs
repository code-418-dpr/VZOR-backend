namespace WhoCame.Visitors.Controllers.Requests;

public record GetFilteredVisitorsWithPaginationRequest(
    string? FirstName,
    string? LastName,
    string? MiddleName,
    string? SortBy,
    string? SortDirection,
    int Page,
    int PageSize);

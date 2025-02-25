using WhoCame.Core.Abstractions;

namespace WhoCame.Visitors.Application.Features.Queries.GetFilteredAndSortedVisitorsWithPagination;

public record GetFilteredAndSortedVisitorsWithPaginationQuery(
    string? FirstName,
    string? LastName,
    string? MiddleName,
    string? SortBy,
    string? SortDirection,
    int Page,
    int PageSize) : IQuery;

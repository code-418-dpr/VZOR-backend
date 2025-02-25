using System.Linq.Expressions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using WhoCame.Core.Abstractions;
using WhoCame.Core.Extension;
using WhoCame.Core.Models;
using WhoCame.SharedKernel;
using WhoCame.Visitors.Application.Database;
using WhoCame.Visitors.Domain;

namespace WhoCame.Visitors.Application.Features.Queries.GetFilteredAndSortedVisitorsWithPagination;

public class GetFilteredAndSortedVisitorsWithPaginationHandler:
    IQueryHandler<PagedList<Visitor>, GetFilteredAndSortedVisitorsWithPaginationQuery>
{
    private readonly IReadDbContext _readDbContext;
    private readonly IValidator<GetFilteredAndSortedVisitorsWithPaginationQuery> _validator;
    private readonly ILogger<GetFilteredAndSortedVisitorsWithPaginationHandler> _logger;

    public GetFilteredAndSortedVisitorsWithPaginationHandler(
        IReadDbContext readDbContext,
        IValidator<GetFilteredAndSortedVisitorsWithPaginationQuery> validator, 
        ILogger<GetFilteredAndSortedVisitorsWithPaginationHandler> logger)
    {
        _readDbContext = readDbContext;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<PagedList<Visitor>>> Handle(
        GetFilteredAndSortedVisitorsWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (validationResult.IsValid == false)
            validationResult.ToErrorList();

        var visitorQuery = _readDbContext.Visitors;
        
        visitorQuery = VisitorQueryFilter(query, visitorQuery);
        
        var keySelector = SortByProperty(query);

        visitorQuery = query.SortDirection?.ToLower() == "desc"
            ? visitorQuery.OrderByDescending(keySelector) 
            : visitorQuery.OrderBy(keySelector);
        
        var pagedList = await visitorQuery.ToPagedList(
            query.Page,
            query.PageSize,
            cancellationToken);
        
        _logger.LogInformation(
            "Get visitors with pagination Page: {Page}, PageSize: {PageSize}, TotalCount: {TotalCount}",
            pagedList.Page, pagedList.PageSize, pagedList.TotalCount);

        return pagedList;
    }
    
    private static Expression<Func<Visitor, object>> SortByProperty(
        GetFilteredAndSortedVisitorsWithPaginationQuery query)
    {
        Expression<Func<Visitor, object>> keySelector = query.SortBy?.ToLower() switch
        {
            "firstName" => (visitor) => visitor.FirstName,
            "lastName" => (visitor) => visitor.LastName,
            "middleName" => (visitor) => visitor.MiddleName ?? visitor.MiddleName!,
            _ => (visitor) => visitor.Id
        };
        return keySelector;
    }

    private static IQueryable<Visitor> VisitorQueryFilter(GetFilteredAndSortedVisitorsWithPaginationQuery query,
        IQueryable<Visitor> visitorQuery)
    {
        visitorQuery = visitorQuery.WhereIf(
            !string.IsNullOrWhiteSpace(query.FirstName),
            v => v.FirstName.Contains(query.FirstName!));
        
        visitorQuery = visitorQuery.WhereIf(
            !string.IsNullOrWhiteSpace(query.LastName),
            v => v.LastName.Contains(query.LastName!));
        
        visitorQuery = visitorQuery.WhereIf(
            !string.IsNullOrWhiteSpace(query.MiddleName),
            v => v.MiddleName!.Contains(query.MiddleName!));
        
        return visitorQuery;
    }
}
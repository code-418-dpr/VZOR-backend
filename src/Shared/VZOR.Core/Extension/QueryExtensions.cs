using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VZOR.Core.Models;

namespace VZOR.Core.Extension;

public static class QueryExtensions
{
    public static async Task<PagedList<T>> ToPagedList<T>(
        this IQueryable<T> source,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await source.CountAsync(cancellationToken);
        
        var items = await source
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        return new PagedList<T>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
    
    public static PagedList<T> ToPagedList<T>(
        this IEnumerable<T> source,
        int page,
        int pageSize)
    {
        var totalCount = source.Count();
    
        var items = source
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    
        return new PagedList<T>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
    
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, bool>> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }
}
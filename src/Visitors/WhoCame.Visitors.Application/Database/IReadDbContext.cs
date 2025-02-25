using WhoCame.Visitors.Domain;

namespace WhoCame.Visitors.Application.Database;

public interface IReadDbContext
{
    IQueryable<Visitor> Visitors { get; }
}
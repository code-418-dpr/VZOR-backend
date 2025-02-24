using WhoCame.SharedKernel;
using WhoCame.Visitors.Domain;

namespace WhoCame.Visitors.Application.Repositories;

public interface IVisitorRepository
{
    Task AddAsync(Visitor visitor, CancellationToken cancellationToken = default);
    Task<Visitor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result>  DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
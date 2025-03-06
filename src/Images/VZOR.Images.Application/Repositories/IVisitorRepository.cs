using VZOR.Images.Domain;
using VZOR.SharedKernel;

namespace VZOR.Images.Application.Repositories;

public interface IVisitorRepository
{
    Task AddAsync(Visitor visitor, CancellationToken cancellationToken = default);
    Task<Visitor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result>  DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
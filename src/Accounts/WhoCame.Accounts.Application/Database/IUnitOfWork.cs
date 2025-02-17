using System.Data;

namespace WhoCame.Accounts.Application.Database;

public interface IUnitOfWork
{
    Task<IDbTransaction> BeginTransaction(CancellationToken cancellationToken = default);
    Task SaveChanges(CancellationToken cancellationToken = default);
}
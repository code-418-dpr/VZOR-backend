using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using WhoCame.Core.Database;

namespace WhoCame.Visitors.Infrastructure;

public class UnitOfWork: IUnitOfWork
{
    private readonly VisitorsDbContext _dbContext;

    public UnitOfWork(VisitorsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IDbTransaction> BeginTransaction(CancellationToken cancellationToken = default)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        return transaction.GetDbTransaction();
    }

    public async Task SaveChanges(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
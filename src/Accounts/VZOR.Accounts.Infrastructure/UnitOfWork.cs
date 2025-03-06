using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using VZOR.Core.Database;

namespace VZOR.Accounts.Infrastructure;

public class UnitOfWork: IUnitOfWork
{
    private readonly AccountsDbContext _dbContext;

    public UnitOfWork(AccountsDbContext dbContext)
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
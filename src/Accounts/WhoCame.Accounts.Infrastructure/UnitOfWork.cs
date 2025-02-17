using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using WhoCame.Accounts.Application.Database;

namespace WhoCame.Accounts.Infrastructure;

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
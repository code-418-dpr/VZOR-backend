using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using WhoCame.Core.Database;

namespace WhoCame.Visitors.Infrastructure;

public class UnitOfWork: IUnitOfWork
{
    private readonly VisitorsWriteDbContext _writeDbContext;

    public UnitOfWork(VisitorsWriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }
    
    public async Task<IDbTransaction> BeginTransaction(CancellationToken cancellationToken = default)
    {
        var transaction = await _writeDbContext.Database.BeginTransactionAsync(cancellationToken);

        return transaction.GetDbTransaction();
    }

    public async Task SaveChanges(CancellationToken cancellationToken = default)
    {
        await _writeDbContext.SaveChangesAsync(cancellationToken);
    }
}
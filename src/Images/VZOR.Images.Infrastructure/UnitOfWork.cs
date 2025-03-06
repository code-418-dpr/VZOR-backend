using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using VZOR.Core.Database;

namespace VZOR.Images.Infrastructure;

public class UnitOfWork: IUnitOfWork
{
    private readonly ApplicationDbContext _writeDbContext;

    public UnitOfWork(ApplicationDbContext writeDbContext)
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
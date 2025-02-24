using Microsoft.EntityFrameworkCore;
using WhoCame.SharedKernel;
using WhoCame.SharedKernel.Errors;
using WhoCame.Visitors.Application.Repositories;
using WhoCame.Visitors.Domain;

namespace WhoCame.Visitors.Infrastructure.Repository;

public class VisitorRepository(VisitorsDbContext context): IVisitorRepository
{
    public async Task AddAsync(Visitor visitor, CancellationToken cancellationToken = default)
    {
        await context.AddAsync(visitor, cancellationToken);
    }

    public async Task<Visitor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var visitor = await context.Visitors.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
        
        return visitor;
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var visitor = await GetByIdAsync(id, cancellationToken);
        if (visitor is null)
            return Errors.General.NotFound(id);

        context.Visitors.Remove(visitor);
        return Result.Success();
    }
}
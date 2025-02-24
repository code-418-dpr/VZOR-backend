using Extensions.Hosting.AsyncInitialization;
using Microsoft.EntityFrameworkCore;

namespace WhoCame.Visitors.Infrastructure;

public class DbInitializer(VisitorsDbContext dbContext) : IAsyncInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}
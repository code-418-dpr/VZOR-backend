using Extensions.Hosting.AsyncInitialization;
using Microsoft.EntityFrameworkCore;

namespace WhoCame.Visitors.Infrastructure;

public class DbInitializer(VisitorsWriteDbContext writeDbContext, VisitorsReadDbContext readDbContext) : IAsyncInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await writeDbContext.Database.MigrateAsync(cancellationToken);
    }
}
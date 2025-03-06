using Extensions.Hosting.AsyncInitialization;
using Microsoft.EntityFrameworkCore;

namespace VZOR.Images.Infrastructure;

public class DbInitializer(ApplicationDbContext dbContext) : IAsyncInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}
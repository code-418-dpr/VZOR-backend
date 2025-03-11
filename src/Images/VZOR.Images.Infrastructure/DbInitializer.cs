using Extensions.Hosting.AsyncInitialization;
using Microsoft.EntityFrameworkCore;
using VZOR.Images.Infrastructure.Contexts;

namespace VZOR.Images.Infrastructure;

public class DbInitializer(WriteDbContext dbContext) : IAsyncInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}
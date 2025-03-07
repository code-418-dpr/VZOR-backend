using Extensions.Hosting.AsyncInitialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VZOR.Accounts.Infrastructure.Seeding;

namespace VZOR.Accounts.Infrastructure;

public class DbInitializer(AccountsDbContext dbContext, IServiceScopeFactory serviceScopeFactory) : IAsyncInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        if (!await dbContext.Database.CanConnectAsync(cancellationToken))
        {
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        }
        
        await dbContext.Database.MigrateAsync(cancellationToken);
        
        using var scope = serviceScopeFactory.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<AccountSeedService>();

        await service.SeedAsync();
    }
}
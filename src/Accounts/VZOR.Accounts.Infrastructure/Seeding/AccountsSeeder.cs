using Microsoft.Extensions.DependencyInjection;

namespace VZOR.Accounts.Infrastructure.Seeding;

public class AccountsSeeder(IServiceScopeFactory serviceScopeFactory)
{
    public async Task SeedAsync()
    {
        using var scope = serviceScopeFactory.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<AccountSeedService>();

        await service.SeedAsync();
    }

}
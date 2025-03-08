using Extensions.Hosting.AsyncInitialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VZOR.Accounts.Infrastructure.Seeding;

namespace VZOR.Accounts.Infrastructure;

public class DbInitializer(
    ILogger<DbInitializer> logger,
    AccountsDbContext dbContext,
    IServiceScopeFactory serviceScopeFactory) : IAsyncInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Применение миграций...");
            if ((await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
            {
                await dbContext.Database.MigrateAsync(cancellationToken);
            }
            
            logger.LogInformation("Заполнение начальными данными...");
            using var scope = serviceScopeFactory.CreateScope();
            var seedService = scope.ServiceProvider.GetRequiredService<AccountSeedService>();
            await seedService.SeedAsync();

            logger.LogInformation("Инициализация базы данных завершена успешно.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при инициализации базы данных.");
            throw; 
        }
    }
}
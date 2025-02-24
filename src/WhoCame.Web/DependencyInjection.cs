using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using WhoCame.Accounts.Application;
using WhoCame.Accounts.Infrastructure;
using WhoCame.Framework.Models;
using WhoCame.Visitors.Infrastructure;

namespace WhoCame.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddModules(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddAccountsManagementModule(configuration)
            .AddVisitorsModule(configuration)
            .AddFramework();
        
        return services;
    }
    
    
    private static IServiceCollection AddFramework(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<UserScopedData>();

        return services;
    }
    
    private static IServiceCollection AddAccountsManagementModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddAccountsApplication()
            .AddAccountsInfrastructure(configuration);
        
        return services;
    }
    
    private static IServiceCollection AddVisitorsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddVisitorsInfrastructure(configuration);
        
        return services;
    }
}
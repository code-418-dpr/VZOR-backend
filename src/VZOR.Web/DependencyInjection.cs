using VZOR.Accounts.Application;
using VZOR.Accounts.Infrastructure;
using VZOR.Framework.Models;
using VZOR.Images.Application;
using VZOR.Images.Infrastructure;

namespace VZOR.Web;

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
            .AddVisitorsInfrastructure(configuration)
            .AddVisitorsApplication();
        
        return services;
    }
}
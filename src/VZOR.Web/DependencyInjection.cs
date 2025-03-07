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
            .AddImagesModule(configuration)
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
    
    private static IServiceCollection AddImagesModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddImagesInfrastructure(configuration)
            .AddImagesApplication();
        
        return services;
    }
}
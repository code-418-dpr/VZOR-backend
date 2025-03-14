using Hangfire;
using Hangfire.PostgreSql;
using ImageGrpc;
using MassTransit;
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
            .AddFramework()
            .AddHangfire(configuration)
            .AddMessageBus(configuration)
            .AddGrpc(configuration);
        
        return services;
    }

    private static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(options =>
        {
            options.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(
                    c =>
                        c.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection")));
        });
        
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
    
    private static IServiceCollection AddMessageBus(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMassTransit(configure =>
        {
            configure.SetKebabCaseEndpointNameFormatter();
            
            configure.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(configuration["RabbitMQ:Host"]!), h =>
                {
                    h.Username(configuration["RabbitMQ:UserName"]!);
                    h.Password(configuration["RabbitMQ:Password"]!);
                });

                cfg.Durable = true;
                
                cfg.ConfigureEndpoints(context);
            });
        });
        
        return services;
    }

    private static IServiceCollection AddGrpc(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpcClient<ImageService.ImageServiceClient>(options =>
        {
            options.Address = new Uri(configuration["Grpc:ImageServiceUrl"]!);
        });
        
        services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = true;
        });

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
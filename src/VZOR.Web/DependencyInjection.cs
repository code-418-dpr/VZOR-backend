using System.IO.Compression;
using System.Net.Security;
using Grpc.Net.Compression;
using Hangfire;
using Hangfire.PostgreSql;
using ImageGrpc;
using Microsoft.AspNetCore.Mvc;
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
                .UsePostgreSqlStorage(c =>
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

    private static IServiceCollection AddGrpc(this IServiceCollection services, IConfiguration configuration)
    {
        var grpcServiceUrl = configuration["Grpc:ImageServiceUrl"];

        services.AddGrpcClient<ImageService.ImageServiceClient>(options =>
            {
                options.Address = new Uri(grpcServiceUrl!);
            })
            .ConfigureChannel(grpcChannelOptions =>
            {
                grpcChannelOptions.HttpHandler = new SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true,
                };

                grpcChannelOptions.UnsafeUseInsecureChannelCallCredentials = true;
            });

        services.AddGrpc(options => { options.EnableDetailedErrors = true; });

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
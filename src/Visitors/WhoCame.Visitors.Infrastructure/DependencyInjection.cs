using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using WhoCame.Core.Database;
using WhoCame.SharedKernel.Constraints;
using WhoCame.Visitors.Application.FileProviders;
using WhoCame.Visitors.Application.Repositories;
using WhoCame.Visitors.Infrastructure.Options;
using WhoCame.Visitors.Infrastructure.Providers;
using WhoCame.Visitors.Infrastructure.Repository;

namespace WhoCame.Visitors.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddVisitorsInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddMinio(configuration)
            .AddDatabase()
            .AddDbContexts()
            .AddRepositories();
        
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IVisitorRepository, VisitorRepository>();

        return services;
    }
    
    private static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddKeyedScoped<IUnitOfWork, UnitOfWork>(Constraints.Contexts.VisitorsContext);
        services.AddKeyedScoped<ISqlConnectionFactory, SqlConnectionFactory>(Constraints.Contexts.VisitorsContext);

        return services;
    }

    private static IServiceCollection AddDbContexts(this IServiceCollection services)
    {
        services.AddScoped<VisitorsDbContext>();

        services.AddAsyncInitializer<DbInitializer>();
        
        return services;
    }
    
    private static IServiceCollection AddMinio(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MinioOptions>(configuration.GetSection(MinioOptions.MINIO));
        
        services.AddMinio(options =>
        {
            var minioOptions = configuration.GetSection(MinioOptions.MINIO)
                .Get<MinioOptions>() ?? throw new ApplicationException("Missing minio configuration");
                
            
            options.WithEndpoint(minioOptions.Endpoint);

            options.WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey);

            options.WithSSL(minioOptions.WithSsl);

        });

        services.AddScoped<IFileProvider, MinioProvider>();

        return services;
    }
}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using WhoCame.Visitors.Application.FileProviders;
using WhoCame.Visitors.Infrastructure.Options;
using WhoCame.Visitors.Infrastructure.Providers;

namespace WhoCame.Visitors.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddVisitorsInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddMinio(configuration);
        
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
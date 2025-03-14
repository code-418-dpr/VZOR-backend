using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;
using MongoDB.Driver;
using VZOR.Core.Database;
using VZOR.Images.Application.Database;
using VZOR.Images.Application.FileProviders;
using VZOR.Images.Application.Repositories;
using VZOR.Images.Infrastructure.Contexts;
using VZOR.Images.Infrastructure.Options;
using VZOR.Images.Infrastructure.Providers;
using VZOR.Images.Infrastructure.Repository;
using VZOR.SharedKernel.Constraints;
using MongoDatabaseSettings = VZOR.Images.Infrastructure.Options.MongoDatabaseSettings;


namespace VZOR.Images.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddImagesInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddMinio(configuration)
            .AddDatabase()
            .AddDbContexts()
            .AddRepositories()
            .AddRedis(configuration)
            .AddMongo(configuration)
            .AddElasticsearch(configuration);
        
        return services;
    }
    
    private static IServiceCollection AddElasticsearch(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ElasticsearchOptions>(
            configuration.GetSection(ElasticsearchOptions.ELASTICSEARCH));
        
        services.AddSingleton<ElasticsearchClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<ElasticsearchOptions>>().Value;

            var settings = new ElasticsearchClientSettings(new Uri(options.Uri))
                .DefaultIndex(options.DefaultIndex);

            return new ElasticsearchClient(settings);
        });

        return services;
    }

    private static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = configuration["Redis:InstanceName"];
        });
        
        
        #pragma warning disable EXTEXP0018
        services.AddHybridCache(options =>
        {
            options.MaximumPayloadBytes = 1024 * 1024 * 10;     
            options.MaximumKeyLength = 512;
         
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(3),
                LocalCacheExpiration = TimeSpan.FromMinutes(3)
            };
        });
    #pragma warning restore EXTEXP0018
        return services;
    }
    
    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddKeyedScoped<IImageRepository, ImageRepository>(Constraints.Database.Postgres);
        services.AddKeyedScoped<IImageRepository, ImageMongoRepository>(Constraints.Database.Mongo);
        services.AddKeyedScoped<IImageRepository, ElasticsearchImageRepository>(Constraints.Database.ElasticSearch);
        
        return services;
    }
    
    private static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddKeyedScoped<IUnitOfWork, UnitOfWork>(Constraints.Contexts.ImagesContext);
        services.AddKeyedScoped<ISqlConnectionFactory, SqlConnectionFactory>(Constraints.Contexts.ImagesContext);

        return services;
    }

    private static IServiceCollection AddMongo(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDatabaseSettings>(
            configuration.GetSection(MongoDatabaseSettings.Mongo) ?? throw new ApplicationException());
        
        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<MongoDatabaseSettings>>().Value;
            return new MongoClient(settings.ConnectionString);
        });
        
        services.AddScoped<IMongoDatabase>(serviceProvider =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<MongoDatabaseSettings>>().Value;
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            return client.GetDatabase(settings.DatabaseName);
        });

        return services;
    }

    private static IServiceCollection AddDbContexts(this IServiceCollection services)
    {
        services.AddScoped<WriteDbContext>();
        services.AddScoped<IReadDbContext, ReadDbContext>();

        //services.AddAsyncInitializer<DbInitializer>();
        
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
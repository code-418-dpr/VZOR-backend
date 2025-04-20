using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace VZOR.Images.Infrastructure;

/// <summary>
/// Сидирование бакета в S3, если он не существует
/// </summary>
/// <param name="fileProvider"></param>
/// <param name="logger"></param>
public class Seeding(IMinioClient fileProvider, ILogger<Seeding> logger)
{
    private const string BUCKET_NAME = "vzor";

    public async Task SeedBucket()
    {
        logger.LogInformation("Start seeding bucket to S3");
        var buckets = await fileProvider.ListBucketsAsync();
        
        if (!buckets.Buckets.Any(b => b.Name.Equals(BUCKET_NAME, StringComparison.OrdinalIgnoreCase)))
        {
            var makeBucket = new MakeBucketArgs()
                .WithBucket(BUCKET_NAME);

            await fileProvider.MakeBucketAsync(makeBucket);
            
            logger.LogInformation("Added bucket {bucketName} to S3", BUCKET_NAME);
        }
        
        logger.LogInformation("End seeding bucket to S3");
    }
    
}
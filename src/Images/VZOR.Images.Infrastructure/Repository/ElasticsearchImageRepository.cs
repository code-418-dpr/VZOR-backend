using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Bulk;
using Elastic.Clients.Elasticsearch.QueryDsl;
using ImageGrpc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using VZOR.Images.Application.Repositories;
using VZOR.Images.Domain;
using VZOR.SharedKernel.Errors;
using MongoDatabaseSettings = VZOR.Images.Infrastructure.Options.MongoDatabaseSettings;
using Result = VZOR.SharedKernel.Result;

namespace VZOR.Images.Infrastructure.Repository;

public class ElasticsearchImageRepository : IImageRepository
{
    private readonly ElasticsearchClient _elasticsearchClient;
    private readonly IMongoCollection<Image> _imageCollection;
    private readonly ILogger<ElasticsearchImageRepository> _logger;

    public ElasticsearchImageRepository(
        IMongoDatabase mongoDatabase,
        IOptions<MongoDatabaseSettings> mongoDatabaseSettings,
        ILogger<ElasticsearchImageRepository> logger,
        ElasticsearchClient elasticsearchClient)
    {
        _elasticsearchClient = elasticsearchClient;
        _imageCollection = mongoDatabase.GetCollection<Image>(mongoDatabaseSettings.Value.ImagesCollectionName);
        _logger = logger;

        CreateIndexIfNotExistsAsync().Wait();
    }

    private async Task CreateIndexIfNotExistsAsync(CancellationToken cancellationToken = default)
    {
        var indexExistsResponse = await _elasticsearchClient.Indices.ExistsAsync("images", cancellationToken);
    
        if (!indexExistsResponse.Exists)
        {
            var createIndexResponse = await _elasticsearchClient.Indices.CreateAsync("images", cancellationToken);

            if (!createIndexResponse.IsValidResponse)
            {
                _logger.LogError($"Failed to create index: {createIndexResponse}");
            }
        }
    }
    
    public async Task AddRangeAsync(IEnumerable<Image> images, CancellationToken cancellationToken = default)
    {
        await _imageCollection.InsertManyAsync(images, cancellationToken: cancellationToken);
        await SynchronizeWithElasticsearchAsync(images, cancellationToken);
    }

    public async Task<Image?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var image = await (await _imageCollection.FindAsync(image => image.Id == id, cancellationToken: cancellationToken))
            .FirstOrDefaultAsync(cancellationToken);
        return image;
    }

    public async Task<Result> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        await _imageCollection.DeleteManyAsync(images => ids.Contains(images.Id), cancellationToken);
        await DeleteFromElasticsearchAsync(ids, cancellationToken);
        return Result.Success();
    }

    public async Task<List<Image>> GetByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        var images = await (await _imageCollection
            .FindAsync(images => ids.Contains(images.Id), cancellationToken: cancellationToken))
            .ToListAsync(cancellationToken);
        return images;
    }

    public async Task<Result> UpdateAsync(UploadImageResponse images, CancellationToken cancellationToken = default)
    {
        try
        {
            var updates = (from image in images.Images
                    let filter = Builders<Image>.Filter.Eq(x => x.Id, image.Id)
                    let update = Builders<Image>.Update.Set(x => x.ProcessingResult.Description, image.Description)
                        .Set(x => x.ProcessingResult.Objects, image.Tags.ToList())
                        .Set(x => x.ProcessingResult.Text, image.RecognizedText)
                    select new UpdateOneModel<Image>(filter, update)).Cast<WriteModel<Image>>()
                .ToList();

            if (updates.Count == 0)
            {
                return Error.Failure("updates.error", "Failed to update images");
            }
            
            await _imageCollection.BulkWriteAsync(updates, cancellationToken: cancellationToken);
                
            var updatedIds = images.Images.Select(i => i.Id).ToList();
            var updatedImages = await GetByIdsAsync(updatedIds, cancellationToken);
                
            await SynchronizeUpdatesWithElasticsearchAsync(updatedImages, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Error.Failure("database.update.error", "Failed to update images");
        }
    }
    
    private async Task SynchronizeUpdatesWithElasticsearchAsync(List<Image> updatedImages, CancellationToken cancellationToken)
    {
        var bulkRequest = new BulkRequest("images")
        {
            Operations = new List<IBulkOperation>()
        };

        foreach (var image in updatedImages)
        {
            bulkRequest.Operations.Add(new BulkIndexOperation<Image>(image)
            {
                Id = image.Id,
                Index = "images"
            });
        }

        var response = await _elasticsearchClient.BulkAsync(bulkRequest, cancellationToken);
    
        if (!response.IsValidResponse)
        {
            _logger.LogError($"Failed to sync updates with Elasticsearch: {response.DebugInformation}");
            throw new Exception("Elasticsearch sync failed");
        }
    }

    public async Task<List<Image>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var images = await (await _imageCollection.FindAsync(
            images => images.UserId == userId, cancellationToken: cancellationToken))
            .ToListAsync(cancellationToken);
        return images;
    }

    public async Task<List<Image>> GetByUserIdWithPaginationAsync(
        string userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Image>> SearchByQueryAsync(string query, CancellationToken cancellationToken = default)
    {
        var request = new SearchRequest<Image>
        {
            Query = new MultiMatchQuery
            {
                Fields = new Field[]
                {
                    Infer.Field<Image>(i => i.ProcessingResult.Description),
                    Infer.Field<Image>(i => i.ProcessingResult.Text),
                    Infer.Field<Image>(i => i.ProcessingResult.Objects.First())
                },
                Query = query.Trim(),
                Type = TextQueryType.BestFields,
                Operator = Operator.Or,
                Fuzziness = new Fuzziness("AUTO")
            }
        };

        var searchResponse = await _elasticsearchClient.SearchAsync<Image>(request, cancellationToken);

        if (!searchResponse.IsValidResponse)
        {
            _logger.LogError($"Search failed: {searchResponse.DebugInformation}");
            return new List<Image>();
        }

        return searchResponse.Documents.ToList();
    }

    private async Task SynchronizeWithElasticsearchAsync(IEnumerable<Image> images, CancellationToken cancellationToken)
    {
        foreach (var image in images)
        {
            await _elasticsearchClient.IndexAsync(image, cancellationToken);
        }
    }

    private async Task DeleteFromElasticsearchAsync(IEnumerable<string> ids, CancellationToken cancellationToken)
    {
        foreach (var id in ids)
        {
            await _elasticsearchClient.DeleteAsync<Image>(id, cancellationToken);
        }
    }
}
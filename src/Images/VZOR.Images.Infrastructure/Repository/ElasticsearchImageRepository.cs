using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using VZOR.Images.Application.Repositories;
using VZOR.Images.Domain;
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
        var searchResponse = await _elasticsearchClient.SearchAsync<Image>(s => s
            .Index("images")
            .Query(q => q
                .Match(m => m
                    .Field(f => f.ProcessingResult) // Замените на нужное поле
                    .Query(query.Trim())
                )
            ), cancellationToken);

        if (!searchResponse.IsValidResponse)
        {
            _logger.LogError($"Search failed: {searchResponse}");
            return new List<Image>(); // Возвращаем пустой список в случае ошибки
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
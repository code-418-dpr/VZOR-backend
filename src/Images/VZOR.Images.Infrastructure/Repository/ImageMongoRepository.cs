using ImageGrpc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using VZOR.Images.Application.Repositories;
using VZOR.Images.Domain;
using VZOR.SharedKernel;
using VZOR.SharedKernel.Errors;
using MongoDatabaseSettings = VZOR.Images.Infrastructure.Options.MongoDatabaseSettings;

namespace VZOR.Images.Infrastructure.Repository;

public class ImageMongoRepository : IImageRepository
{
    private readonly IMongoCollection<Image> _imageCollection;

    public ImageMongoRepository(IMongoDatabase database, IOptions<MongoDatabaseSettings> settings)
    {
        _imageCollection = database.GetCollection<Image>(settings.Value.ImagesCollectionName);
    }

    public async Task AddRangeAsync(IEnumerable<Image> images, CancellationToken cancellationToken = default)
    {
        await _imageCollection.InsertManyAsync(images, cancellationToken: cancellationToken);
    }

    public async Task<Image?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var image = await (await _imageCollection.FindAsync(image =>
                image.Id == id, cancellationToken: cancellationToken))
            .FirstOrDefaultAsync(cancellationToken);

        return image;
    }

    public async Task<Result> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        await _imageCollection.DeleteManyAsync(images => ids.Contains(images.Id), cancellationToken);

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

            if (updates.Count != 0)
            {
                await _imageCollection.BulkWriteAsync(updates, cancellationToken: cancellationToken);
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Error.Failure("database.update.error", "Failed to update images");
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
        var images = await (await _imageCollection.FindAsync(
                images => images.UserId == userId, cancellationToken: cancellationToken))
            .ToListAsync(cancellationToken);

        images = images
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return images;
    }

    public Task<List<Image>> SearchByQueryAsync(string query, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
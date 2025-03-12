using Microsoft.EntityFrameworkCore;
using VZOR.Images.Application.Repositories;
using VZOR.Images.Domain;
using VZOR.Images.Infrastructure.Contexts;
using VZOR.SharedKernel;
using VZOR.SharedKernel.Errors;


namespace VZOR.Images.Infrastructure.Repository;

public class ImageRepository(WriteDbContext context): IImageRepository
{
    public async Task AddRangeAsync(IEnumerable<Image> images, CancellationToken cancellationToken = default)
    {
        await context.AddRangeAsync(images, cancellationToken);
    }

    public async Task<Image?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var image = await context.Images.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
        
        return image;
    }

    public async Task<List<Image>> GetByIdsAsync(IEnumerable<string> ids,CancellationToken cancellationToken = default)
    {
        var images = await context.Images.Where(v => ids.Contains(v.Id)).ToListAsync(cancellationToken);
        
        return images;
    }

    public async Task<Result> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        var images = await GetByIdsAsync(ids, cancellationToken);
        if (!images.Any())
            return Errors.General.NotFound();

        context.Images.RemoveRange(images);
        return Result.Success();
    }

    public async Task<List<Image>> GetByUserIdWithPaginationAsync(
        string userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var images = await context.Images
            .Where(v => v.UserId == userId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        return images;
    }
    
    public async Task<List<Image>> GetByUserIdAsync(
        string userId, CancellationToken cancellationToken = default)
    {
        var images = await context.Images
            .Where(v => v.UserId == userId)
            .ToListAsync(cancellationToken);
        
        return images;
    }
}
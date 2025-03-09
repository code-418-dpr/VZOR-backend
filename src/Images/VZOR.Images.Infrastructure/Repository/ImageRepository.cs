using Microsoft.EntityFrameworkCore;
using VZOR.Images.Application.Repositories;
using VZOR.Images.Domain;
using VZOR.SharedKernel;
using VZOR.SharedKernel.Errors;


namespace VZOR.Images.Infrastructure.Repository;

public class ImageRepository(ApplicationDbContext context): IImageRepository
{
    public async Task AddRangeAsync(IEnumerable<Image> images, CancellationToken cancellationToken = default)
    {
        await context.AddRangeAsync(images, cancellationToken);
    }

    public async Task<Image?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var image = await context.Images.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
        
        return image;
    }

    public async Task<List<Image>> GetByIdsAsync(IEnumerable<Guid> ids,CancellationToken cancellationToken = default)
    {
        var images = await context.Images.Where(v => ids.Contains(v.Id)).ToListAsync(cancellationToken);
        
        return images;
    }

    public async Task<Result> DeleteAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var images = await GetByIdsAsync(ids, cancellationToken);
        if (!images.Any())
            return Errors.General.NotFound();

        context.Images.RemoveRange(images);
        return Result.Success();
    }
}
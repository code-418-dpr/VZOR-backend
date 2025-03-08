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
        var visitor = await context.Images.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
        
        return visitor;
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var visitor = await GetByIdAsync(id, cancellationToken);
        if (visitor is null)
            return Errors.General.NotFound(id);

        context.Images.Remove(visitor);
        return Result.Success();
    }
}
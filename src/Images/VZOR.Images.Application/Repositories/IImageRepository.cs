using VZOR.Images.Domain;
using VZOR.SharedKernel;

namespace VZOR.Images.Application.Repositories;

public interface IImageRepository
{
    Task AddRangeAsync(IEnumerable<Image> images, CancellationToken cancellationToken = default);
    Task<Image?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result>  DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
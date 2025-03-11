using VZOR.Images.Domain;
using VZOR.SharedKernel;

namespace VZOR.Images.Application.Repositories;

public interface IImageRepository
{
    Task AddRangeAsync(IEnumerable<Image> images, CancellationToken cancellationToken = default);
    Task<Image?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result>  DeleteAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<List<Image>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<List<Image>> GetByUserIdAsync(
        Guid userId, CancellationToken cancellationToken = default);
    Task<List<Image>> GetByUserIdWithPaginationAsync(
        Guid userId, int page,int pageSize, CancellationToken cancellationToken = default);
}
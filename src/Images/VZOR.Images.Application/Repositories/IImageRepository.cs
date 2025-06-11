using ImageGrpc;
using VZOR.Images.Domain;
using VZOR.SharedKernel;

namespace VZOR.Images.Application.Repositories;

public interface IImageRepository
{
    Task AddRangeAsync(IEnumerable<Image> images, CancellationToken cancellationToken = default);
    Task<Image?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    Task<List<Image>> GetByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(UploadImageResponse images, CancellationToken cancellationToken = default);
    Task<List<Image>> GetByUserIdAsync(
        string userId, CancellationToken cancellationToken = default);

    Task<List<Image>> GetByUserIdWithPaginationAsync(
        string userId, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<List<Image>> SearchByQueryAsync(string query, CancellationToken cancellationToken = default);
}
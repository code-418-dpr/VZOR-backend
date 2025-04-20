using VZOR.Images.Application.FileModels;
using VZOR.SharedKernel;

namespace VZOR.Images.Application.FileProviders;

public interface IS3FileProvider
{
    Task<Result<string>> GetPresignedUrlForUpload(FileMetadata fileMetadata, CancellationToken cancellationToken);
    Task<Result<IReadOnlyList<string>>> DownloadFiles(
        IEnumerable<FileMetadata> filesMetadata, CancellationToken cancellationToken = default);
    Task<Result<List<string>>> GetPresignedUrlsForDeleteParallel(
        IEnumerable<FileMetadata> fileMetadata, CancellationToken cancellationToken);
    Task DeleteFile(FileMetadata fileMetadata, CancellationToken cancellationToken = default);
    Task<Result<List<string>>> GetPresignedUrlsForUploadParallel(
        IEnumerable<FileMetadata> fileMetadata,
        CancellationToken cancellationToken = default);
}
using VZOR.Images.Application.FileModels;
using VZOR.SharedKernel;

namespace VZOR.Images.Application.FileProviders;

public interface IS3FileProvider
{
    Task<Result<string>> GetPresignedUrlForUpload(FileMetadataS3 fileMetadata, CancellationToken cancellationToken);
    Task<Result<IReadOnlyList<string>>> DownloadFiles(
        IEnumerable<FileMetadataS3> filesMetadata, CancellationToken cancellationToken = default);
    Task<Result<List<string>>> GetPresignedUrlsForDeleteParallel(
        IEnumerable<FileMetadataS3> fileMetadata, CancellationToken cancellationToken);
    Task DeleteFile(FileMetadataS3 fileMetadata, CancellationToken cancellationToken = default);
    Task<Result<List<string>>> GetPresignedUrlsForUploadParallel(
        IEnumerable<FileMetadataS3> fileMetadata,
        CancellationToken cancellationToken = default);
    
}
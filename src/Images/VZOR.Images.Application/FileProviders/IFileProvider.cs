using VZOR.Images.Application.FileModels;
using VZOR.SharedKernel;
using FileInfo = VZOR.Images.Application.FileModels.FileInfo;

namespace VZOR.Images.Application.FileProviders;

public interface IFileProvider
{
    Task<Result<IReadOnlyList<string>>> UploadFiles(
        IEnumerable<FileData> filesData, CancellationToken cancellationToken = default);
    Task<Result<string>> DeleteFile(FileMetadata fileMetadata, CancellationToken cancellationToken = default);
    Task<Result<string>> GetFileByObjectName(FileMetadata fileMetadata, CancellationToken cancellationToken = default);
    Task<Result<List<string>>> DownloadFilesByPresignedUrls(
        IEnumerable<FileMetadata> files, CancellationToken cancellationToken = default);
    Task<Result> RemoveFile(FileInfo fileInfo, CancellationToken cancellationToken = default);
}
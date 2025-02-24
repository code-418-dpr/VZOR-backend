using WhoCame.SharedKernel;
using WhoCame.Visitors.Application.FileProvider;
using FileInfo = WhoCame.Visitors.Application.FileProvider.FileInfo;

namespace WhoCame.Visitors.Application.FileProviders;

public interface IFileProvider
{
    Task<Result<IReadOnlyList<string>>> UploadFiles(IEnumerable<FileData> filesData, CancellationToken cancellationToken);
    Task<Result<string>> DeleteFile(FileMetadata fileMetadata, CancellationToken cancellationToken);
    Task<Result<string>> GetFileByObjectName(FileMetadata fileMetadata, CancellationToken cancellationToken);
    Task<Result> RemoveFile(FileInfo fileInfo, CancellationToken cancellationToken);
}
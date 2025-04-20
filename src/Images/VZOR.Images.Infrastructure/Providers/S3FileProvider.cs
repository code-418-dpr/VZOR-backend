using Amazon.S3;
using Microsoft.Extensions.Logging;
using VZOR.Images.Application.FileModels;
using VZOR.Images.Application.FileProviders;
using VZOR.SharedKernel;

namespace VZOR.Images.Infrastructure.Providers;

public class S3FileProvider: IS3FileProvider
{
    private const int MAX_DEGREE_OF_PARALLELISM = 50;
    private const int EXPIRATION_URL = 1;
    private readonly IAmazonS3 _client;
    private readonly ILogger<MinioProvider> _logger;
    
    public async Task<Result<string>> GetPresignedUrlForUpload(
        FileMetadata fileMetadata, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<IReadOnlyList<string>>> DownloadFiles(
        IEnumerable<FileMetadata> filesMetadata, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<List<string>>> GetPresignedUrlsForDeleteParallel(
        IEnumerable<FileMetadata> fileMetadata, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteFile(
        FileMetadata fileMetadata, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<List<string>>> GetPresignedUrlsForUploadParallel(
        IEnumerable<FileMetadata> fileMetadata, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
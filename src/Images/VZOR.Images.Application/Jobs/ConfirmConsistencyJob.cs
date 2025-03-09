using Hangfire;
using Microsoft.Extensions.Logging;
using VZOR.Images.Application.FileProvider;
using VZOR.Images.Application.FileProviders;
using VZOR.Images.Application.Repositories;

namespace VZOR.Images.Application.Jobs;

public class ConfirmConsistencyJob(
    IImageRepository repository,
    IFileProvider fileProvider,
    ILogger<ConfirmConsistencyJob> logger)
{
    [AutomaticRetry(Attempts = 3, DelaysInSeconds = [5, 10, 15])]
    public async Task Execute(IEnumerable<Guid> imageIds,string bucketName, IEnumerable<string> uploadUrls)
    {
        try
        {
            logger.LogInformation("Start ConfirmConsistencyJob");

            List<string> metaDatas = [];

            foreach (var url in uploadUrls)
            {
                var fileMetaData = new FileMetadata(bucketName, url);
                var metadata = await fileProvider.GetFileByObjectName(fileMetaData);
                if (metadata.IsSuccess)
                    metaDatas.Add(metadata.Value);
                else
                    logger.LogWarning("Metadata not found for file: {fileUrl}", url);
            }

            var images = await repository.GetByIdsAsync(imageIds);

            if (images.Count < imageIds.Count())
            {
                logger.LogWarning("Some Postgres records not found for the given fileIds." +
                                  " Deleting all files from cloud storage.");
                
                var deleteTasks = metaDatas.Select(m =>
                    fileProvider.DeleteFile(new FileMetadata(bucketName, m)));
                
                await Task.WhenAll(deleteTasks);

                await repository.DeleteAsync(imageIds);
                return;
            }

            var isConsistent = true;
            if ((from metadata in metaDatas 
                    let image = images.FirstOrDefault(i => i.UploadLink == metadata) 
                    where image == null || metadata != image.UploadLink 
                    select metadata).Any())
            {
                logger.LogWarning("Metadata key does not match Postgres data." +
                                  " Deleting file from cloud storage and Postgres record.");
                
                isConsistent = false;
            }

            if (!isConsistent)
            {
                var deleteTasks = metaDatas.Select(m =>
                    fileProvider.DeleteFile(new FileMetadata(bucketName, m)));
                
                await Task.WhenAll(deleteTasks);
                
                await repository.DeleteAsync(imageIds);
            }

            logger.LogInformation("End ConfirmConsistencyJob");
        }
        catch (Exception ex)
        {
            logger.LogError("Cannot check consistency, because " + ex.Message);
        }
    }
}
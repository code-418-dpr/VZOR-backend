using FluentValidation;
using Grpc.Core;
using ImageGrpc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VZOR.Core.Abstractions;
using VZOR.Core.Extension;
using VZOR.Images.Application.FileModels;
using VZOR.Images.Application.FileProviders;
using VZOR.Images.Application.Repositories;
using VZOR.Images.Domain;
using VZOR.SharedKernel;
using VZOR.SharedKernel.Constraints;
using VZOR.SharedKernel.Errors;

namespace VZOR.Images.Application.Features.Commands.UploadImage;

public class ProcessImageHandler : ICommandHandler<ProcessImageCommand>
{
    public const string BUCKET_NAME = "vzor";

    private readonly ILogger<ProcessImageHandler> _logger;
    private readonly IValidator<ProcessImageCommand> _validator;
    private readonly IImageRepository _repository;
    private readonly IS3FileProvider _s3FileProvider;
    private readonly ImageService.ImageServiceClient _grpcClient;

    public ProcessImageHandler(
        ILogger<ProcessImageHandler> logger,
        IValidator<ProcessImageCommand> validator,
        [FromKeyedServices(Constraints.Database.Mongo)]
        IImageRepository repository,
        ImageService.ImageServiceClient grpcClient,
        IS3FileProvider s3FileProvider)
    {
        _logger = logger;
        _validator = validator;
        _repository = repository;
        _grpcClient = grpcClient;
        _s3FileProvider = s3FileProvider;
    }

    public async Task<Result> Handle(ProcessImageCommand command, CancellationToken cancellationToken = default)
    {
        var validatorResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validatorResult.IsValid)
            return validatorResult.ToErrorList();

        try
        {
            var images =
                await _repository.GetByIdsAsync(command.FileIds.Select(id => id.ToString()), cancellationToken);

            await SendImagesByGrpc(cancellationToken, images);

            var ids = images.Select(x => x.Id).ToList();
            var uploadLinks = images.Select(x => x.UploadLink).ToList();

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            return Error.Failure("upload.photos.error", "Cannot upload photos");
        }
    }

    private async Task SendImagesByGrpc(CancellationToken cancellationToken, List<Image> images)
    {
        try
        {
            var uploadImageRequest = new UploadImageRequest();

            var fileNames = images.Select(x => x.UploadLink).ToList();
            var contentTypes = images
                .Select(i => GetContentTypeFromExtension(Path.GetExtension(i.UploadLink)))
                .ToList();

            var metadatas = fileNames
                .Select((t, i) => new FileMetadataS3
                {
                    BucketName = BUCKET_NAME,
                    ContentType = contentTypes[i],
                    Key = t
                })
                .ToList();

            var presignedUrlsForDownload = await _s3FileProvider
                .DownloadFiles(metadatas, cancellationToken);

            if (presignedUrlsForDownload.IsFailure)
            {
                throw new Exception("presignedUrlsForDownload.IsFailure");
            }

            for (var i = 0; i < images.Count; i++)
            {
                uploadImageRequest.Images.Add(new ImageToProcess
                {
                    Id = images[i].Id,
                    Url = presignedUrlsForDownload.Value[i],
                });
            }
            
            var response = await _grpcClient.UploadImageAsync(uploadImageRequest);
            
            

            _logger.LogInformation("got proccessed images");
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("Timeout occurred while uploading images via gRPC");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while uploading images via gRPC");
        }
    }

    private string GetContentTypeFromExtension(string extension)
    {
        var formattedExtension = extension.Replace(".", "");

        return formattedExtension.Equals("jpg", StringComparison.InvariantCultureIgnoreCase)
            ? "image/jpeg"
            : $"image/{formattedExtension}";
    }
}
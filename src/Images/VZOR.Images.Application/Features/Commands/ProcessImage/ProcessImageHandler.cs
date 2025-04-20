using FluentValidation;
using Google.Protobuf;
using Hangfire;
using ImageGrpc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VZOR.Core.Abstractions;
using VZOR.Core.Database;
using VZOR.Core.Extension;
using VZOR.Images.Application.FileModels;
using VZOR.Images.Application.FileProviders;
using VZOR.Images.Application.Jobs;
using VZOR.Images.Application.Repositories;
using VZOR.Images.Domain;
using VZOR.SharedKernel;
using VZOR.SharedKernel.Constraints;
using VZOR.SharedKernel.Errors;
using FileInfo = VZOR.Images.Application.FileModels.FileInfo;

namespace VZOR.Images.Application.Features.Commands.UploadImage;

public class ProcessImageHandler: ICommandHandler<ProcessImageCommand>
{
    public const string BUCKET_NAME = "vzor";
    
    private readonly ILogger<ProcessImageHandler> _logger;
    private readonly IValidator<ProcessImageCommand> _validator;
    private readonly IImageRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileProvider _fileProvider;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ImageService.ImageServiceClient _grpcClient;

    public ProcessImageHandler(
        ILogger<ProcessImageHandler> logger,
        IValidator<ProcessImageCommand> validator,
        [FromKeyedServices(Constraints.Database.ElasticSearch)]IImageRepository repository,
        [FromKeyedServices(Constraints.Contexts.ImagesContext)]IUnitOfWork unitOfWork,
        IFileProvider fileProvider, 
        IDateTimeProvider dateTimeProvider,
        ImageService.ImageServiceClient grpcClient)
    {
        _logger = logger;
        _validator = validator;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _fileProvider = fileProvider;
        _dateTimeProvider = dateTimeProvider;
        _grpcClient = grpcClient;
    }

    public async Task<Result> Handle(ProcessImageCommand command, CancellationToken cancellationToken = default)
    {
        var validatorResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validatorResult.IsValid)
            return validatorResult.ToErrorList();

        try
        {
            List<FileData> filesData = [];
            List<Image> images = [];
            foreach (var image in command.Images)
            {
                var id = Guid.NewGuid();

                var uploadLink = id + Path.GetExtension(image.FileName);
                
                var file = new FileData(image.Content,
                    new FileInfo(uploadLink, BUCKET_NAME));
                
                filesData.Add(file);
                
                images.Add(new Image
                {
                    Id = id.ToString(),
                    UserId = command.UserId.ToString(),
                    UploadLink = uploadLink,
                    UploadDate = _dateTimeProvider.UtcNow
                });
            }

            await _repository.AddRangeAsync(images, cancellationToken);
            
            var paths = await _fileProvider.UploadFiles(filesData, cancellationToken);
            if (paths.IsFailure)
                return paths.Errors;
            
            _logger.LogInformation("Uploaded photos by user with user id {userId}", command.UserId);
            
            await SendImagesByGrpc(cancellationToken, images, filesData);
            
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

    private async Task SendImagesByGrpc(CancellationToken cancellationToken, List<Image> images, List<FileData> filesData)
    {
        TimeSpan timeout = TimeSpan.FromSeconds(15);
        
        foreach (var image in images)
        {
            var fileData = filesData.FirstOrDefault(f => f.FileInfo.FilePath == image.UploadLink);
            
            if (fileData == null) continue;

            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(timeout);

                await using var memoryStream = new MemoryStream();

                await fileData.Stream.CopyToAsync(memoryStream, cancellationToken);

                var byteArray = memoryStream.ToArray();

                var byteString = ByteString.CopyFrom(byteArray);

                var request = new UploadImageRequest
                {
                    ImageId = image.Id,
                    ImageData = byteString
                };

                var response = await _grpcClient.UploadImageAsync(request, cancellationToken: cts.Token);
                if (!response.Success)
                {
                    _logger.LogError("Failed to upload image {ImageId} via gRPC: {Message}", image.Id,
                        response.Message);
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("Timeout occurred while uploading image {ImageId} via gRPC", image.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading image {ImageId} via gRPC", image.Id);
            }
        }
    }
}
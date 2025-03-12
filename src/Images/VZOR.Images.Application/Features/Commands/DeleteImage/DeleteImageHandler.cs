using FluentValidation;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VZOR.Core.Abstractions;
using VZOR.Core.Extension;
using VZOR.Images.Application.FileProvider;
using VZOR.Images.Application.FileProviders;
using VZOR.Images.Application.Jobs;
using VZOR.Images.Application.Repositories;
using VZOR.SharedKernel;
using VZOR.SharedKernel.Constraints;
using VZOR.SharedKernel.Errors;

namespace VZOR.Images.Application.Features.Commands.DeleteImage;

public class DeleteImageHandler: ICommandHandler<DeleteImageCommand>
{
    public const string BUCKET_NAME = "vzor";
    
    private readonly ILogger<DeleteImageHandler> _logger;
    private readonly IValidator<DeleteImageCommand> _validator;
    private readonly IImageRepository _imageRepository;
    private readonly IFileProvider _fileProvider;

    public DeleteImageHandler(
        ILogger<DeleteImageHandler> logger,
        IValidator<DeleteImageCommand> validator,
        [FromKeyedServices(Constraints.Database.Mongo)]IImageRepository imageRepository,
        IFileProvider fileProvider)
    {
        _logger = logger;
        _validator = validator;
        _imageRepository = imageRepository;
        _fileProvider = fileProvider;
    }

    public async Task<Result> Handle(DeleteImageCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrorList();
        
        var image = await _imageRepository.GetByIdAsync(command.ImageId.ToString(), cancellationToken);

        if (image is null || image.UserId != command.UserId.ToString())
            return Errors.General.NotFound();

        var fileMetadata = new FileMetadata(BUCKET_NAME, image.UploadLink);

        await _imageRepository.DeleteAsync([command.ImageId.ToString()], cancellationToken);
        
        var result = await _fileProvider.DeleteFile(fileMetadata, cancellationToken);
        if (result.IsFailure)
            return result.Errors;
        
        _logger.LogInformation("Deleted photos by user with user id {userId}", command.UserId);

        var jobId = BackgroundJob.Schedule<ConfirmConsistencyJob>(
            j => j.Execute(
                new[] { command.ImageId.ToString() }, BUCKET_NAME, new[] { image.UploadLink }),
            TimeSpan.FromMinutes(1));
            
        return Result.Success();
    }
}
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VZOR.Core.Abstractions;
using VZOR.Core.Database;
using VZOR.Core.Extension;
using VZOR.Images.Application.FileProvider;
using VZOR.Images.Application.FileProviders;
using VZOR.Images.Application.Repositories;
using VZOR.Images.Domain;
using VZOR.SharedKernel;
using VZOR.SharedKernel.Constraints;
using VZOR.SharedKernel.Errors;
using FileInfo = VZOR.Images.Application.FileProvider.FileInfo;

namespace VZOR.Images.Application.Features.UploadImage;

public class UploadImageHandler: ICommandHandler<UploadImageCommand>
{
    public const string BUCKET_NAME = "vzor";
    
    private readonly ILogger<UploadImageHandler> _logger;
    private readonly IValidator<UploadImageCommand> _validator;
    private readonly IImageRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileProvider _fileProvider;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UploadImageHandler(
        ILogger<UploadImageHandler> logger,
        IValidator<UploadImageCommand> validator,
        IImageRepository repository,
        [FromKeyedServices(Constraints.Contexts.ImagesContext)]IUnitOfWork unitOfWork,
        IFileProvider fileProvider, 
        IDateTimeProvider dateTimeProvider)
    {
        _logger = logger;
        _validator = validator;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _fileProvider = fileProvider;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result> Handle(UploadImageCommand command, CancellationToken cancellationToken = default)
    {
        var validatorResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validatorResult.IsValid)
            return validatorResult.ToErrorList();
        
        var transaction = await _unitOfWork.BeginTransaction(cancellationToken);

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
                    Id = id,
                    UserId = command.UserId,
                    UploadLink = uploadLink,
                    UploadDate = _dateTimeProvider.UtcNow,
                });
            }

            await _repository.AddRangeAsync(images, cancellationToken);
            
            var paths = await _fileProvider.UploadFiles(filesData, cancellationToken);
            if (paths.IsFailure)
                return paths.Errors;
            
            await _unitOfWork.SaveChanges(cancellationToken);
            
            transaction.Commit();
            
            _logger.LogInformation("Uploaded photos by user with user id {userId}", command.UserId);
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            
            _logger.LogError(ex, ex.Message);

            return Error.Failure("upload.photos.error", "Cannot upload photos");
        }
    }
}
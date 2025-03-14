using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VZOR.Core.Abstractions;
using VZOR.Core.Dtos;
using VZOR.Core.Extension;
using VZOR.Images.Application.FileModels;
using VZOR.Images.Application.FileProviders;
using VZOR.Images.Application.Repositories;
using VZOR.Images.Domain;
using VZOR.SharedKernel;
using VZOR.SharedKernel.Constraints;
using VZOR.SharedKernel.Errors;

namespace VZOR.Images.Application.Features.Queries.GetImageById;

public class GetImageByIdHandler: IQueryHandler<ImageDto, GetImageByIdQuery>
{
    public const string BUCKET_NAME = "vzor";
    
    private readonly IImageRepository _imageRepository;
    private readonly ILogger<GetImageByIdHandler> _logger;
    private readonly IValidator<GetImageByIdQuery> _validator;
    private readonly IFileProvider _fileProvider;

    public GetImageByIdHandler(
        [FromKeyedServices(Constraints.Database.Mongo)]IImageRepository imageRepository,
        ILogger<GetImageByIdHandler> logger,
        IValidator<GetImageByIdQuery> validator,
        IFileProvider fileProvider)
    {
        _imageRepository = imageRepository;
        _logger = logger;
        _validator = validator;
        _fileProvider = fileProvider;
    }

    public async Task<Result<ImageDto>> Handle(GetImageByIdQuery query, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrorList();
        
        var image = await _imageRepository.GetByIdAsync(query.ImageId.ToString(), cancellationToken);
        if (image is null)
            return Errors.General.NotFound();

        var presignedUrl = await _fileProvider
            .DownloadFilesByPresignedUrls([new FileMetadata(BUCKET_NAME, image.UploadLink)], cancellationToken);

        if (presignedUrl.IsFailure)
            return Errors.General.NotFound();

        var imageDto = new ImageDto
        {
            Id = image.Id,
            ProcessingResult = new ProcessingResultDto
            {
                Description  = image.ProcessingResult.Description,
                Objects = image.ProcessingResult.Objects,
                Text = image.ProcessingResult.Text
            },
            UploadDate = image.UploadDate,
            UserId = image.UserId,
            PresignedDownloadUrl = presignedUrl.Value.FirstOrDefault()!,
            UploadLink = image.UploadLink
        };
        
        _logger.LogInformation($"Get image with id: {query.ImageId}");
        
        return imageDto;
    }
}
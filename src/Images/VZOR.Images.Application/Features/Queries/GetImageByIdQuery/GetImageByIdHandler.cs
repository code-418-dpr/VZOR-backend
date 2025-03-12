using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VZOR.Core.Abstractions;
using VZOR.Core.Extension;
using VZOR.Images.Application.Repositories;
using VZOR.Images.Domain;
using VZOR.SharedKernel;
using VZOR.SharedKernel.Constraints;
using VZOR.SharedKernel.Errors;

namespace VZOR.Images.Application.Features.Queries.GetImageByIdQuery;

public class GetImageByIdHandler: IQueryHandler<Image, GetImageByIdQuery>
{
    private readonly IImageRepository _imageRepository;
    private readonly ILogger<GetImageByIdHandler> _logger;
    private readonly IValidator<GetImageByIdQuery> _validator;

    public GetImageByIdHandler(
        [FromKeyedServices(Constraints.Database.Mongo)]IImageRepository imageRepository,
        ILogger<GetImageByIdHandler> logger,
        IValidator<GetImageByIdQuery> validator)
    {
        _imageRepository = imageRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Image>> Handle(GetImageByIdQuery query, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrorList();
        
        var image = await _imageRepository.GetByIdAsync(query.ImageId.ToString(), cancellationToken);
        if (image is null)
            return Errors.General.NotFound();

        _logger.LogInformation($"Get image with id: {query.ImageId}");
        
        return image;
    }
}
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VZOR.Core.Abstractions;
using VZOR.Core.Extension;
using VZOR.Images.Application.Repositories;
using VZOR.Images.Domain;
using VZOR.SharedKernel;
using VZOR.SharedKernel.Constraints;

namespace VZOR.Images.Application.Features.Queries.GetImagesByQuery;

public class GetImagesByQueryHandler: IQueryHandler<List<Image>, GetImagesByQueryQuery>
{
    private readonly ILogger<GetImagesByQueryHandler> _logger;
    private readonly IValidator<GetImagesByQueryQuery> _validator;
    private readonly IImageRepository _imageRepository;


    public GetImagesByQueryHandler(
        ILogger<GetImagesByQueryHandler> logger, 
        IValidator<GetImagesByQueryQuery> validator,
        [FromKeyedServices(Constraints.Database.ElasticSearch)]IImageRepository imageRepository)
    {
        _logger = logger;
        _validator = validator;
        _imageRepository = imageRepository;
    }

    public async Task<Result<List<Image>>> Handle(
        GetImagesByQueryQuery query, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrorList();

        var result = await _imageRepository.SearchByQueryAsync(query.Query, cancellationToken);

        _logger.LogInformation("Get Images by query {query}: {UserId}", query.UserId, query.Query);
        
        return result;
    }
}
using System.Linq.Expressions;
using FluentValidation;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VZOR.Core.Abstractions;
using VZOR.Core.Dtos;
using VZOR.Core.Extension;
using VZOR.Core.Models;
using VZOR.Images.Application.FileProvider;
using VZOR.Images.Application.FileProviders;
using VZOR.Images.Application.Repositories;
using VZOR.Images.Domain;
using VZOR.SharedKernel;
using VZOR.SharedKernel.Constraints;

namespace VZOR.Images.Application.Features.Queries.GetImages;

public class GetImagesHandler : IQueryHandler<PagedList<ImageDto>, GetImagesQuery>
{
    public const string BUCKET_NAME = "vzor";
    
    private readonly ILogger<GetImagesHandler> _logger;
    private readonly IValidator<GetImagesQuery> _validator;
    private readonly HybridCache _hybridCache;
    private readonly IImageRepository _imageRepository;
    private readonly IFileProvider _fileProvider;

    public GetImagesHandler(
        ILogger<GetImagesHandler> logger,
        IValidator<GetImagesQuery> validator,
        HybridCache hybridCache,
        [FromKeyedServices(Constraints.Database.Mongo)]IImageRepository imageRepository,
        IFileProvider fileProvider)
    {
        _logger = logger;
        _validator = validator;
        _hybridCache = hybridCache;
        _imageRepository = imageRepository;
        _fileProvider = fileProvider;
    }

    public async Task<Result<PagedList<ImageDto>>> Handle(
        GetImagesQuery query, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrorList();
        
        var imagesCahce = await _hybridCache.GetOrCreateAsync(
            $"images_{query.UserId}", 
            async _ =>
            {
                var images = await _imageRepository
                    .GetByUserIdAsync(query.UserId.ToString(), cancellationToken);
                
                return images;
            },
            cancellationToken: cancellationToken);

        if (imagesCahce.Count == 0)
            return new PagedList<ImageDto>();

        var images = imagesCahce.AsQueryable();
        
        images = ImagesQueryFilter(query, images);

        var keySelector = SortByProperty(query);

        images = query.SortDirection?.ToLower() == "desc"
            ? images.OrderByDescending(keySelector) 
            : images.OrderBy(keySelector);

        var presignedUrls = await _fileProvider.DownloadFilesByPresignedUrls(images
            .Select(i => new FileMetadata(BUCKET_NAME, i.UploadLink)), cancellationToken);

        var imagesDto = images
            .Zip(presignedUrls.Value, (image, presignedUrl) => new ImageDto
            {
                Id = image.Id,
                ProcessingResult = image.ProcessingResult,
                UploadDate = image.UploadDate,
                UserId = image.UserId,
                PresignedDownloadUrl = presignedUrl ,
                UploadLink = image.UploadLink
            })
            .ToList();
        
        var pagedList = imagesDto.ToPagedList(
            query.Page,
            query.PageSize);
        
        _logger.LogInformation("Get Images Query: {UserId}", query.UserId);
        
        return pagedList;
    }
    
    private static Expression<Func<Image, object>> SortByProperty(GetImagesQuery query)
    {
        Expression<Func<Image, object>> keySelector = query.SortBy?.ToLower() switch
        {
            "UploadDate" => (image) => image.UploadDate,
            _ => (image) => image.Id
        };
        return keySelector;
    }
    
    private static IQueryable<Image> ImagesQueryFilter(GetImagesQuery query,
        IQueryable<Image> imagesQuery)
    {
        imagesQuery = imagesQuery.WhereIf(
            query.StartUploadDate != null,
            i => i.UploadDate >= query.StartUploadDate);
        
        imagesQuery = imagesQuery.WhereIf(
            query.EndUploadDate != null,
            i => i.UploadDate <= query.EndUploadDate);
        
        return imagesQuery;
    }
}
using System.Linq.Expressions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using VZOR.Core.Abstractions;
using VZOR.Core.Extension;
using VZOR.Core.Models;
using VZOR.Images.Application.Database;
using VZOR.Images.Application.Repositories;
using VZOR.Images.Domain;
using VZOR.SharedKernel;

namespace VZOR.Images.Application.Features.Queries.GetImagesQuery;

public class GetImagesHandler : IQueryHandler<PagedList<Image>, GetImagesQuery>
{
    private readonly ILogger<GetImagesHandler> _logger;
    private readonly IValidator<GetImagesQuery> _validator;
    private readonly HybridCache _hybridCache;
    private readonly IReadDbContext _readDbContext;

    public GetImagesHandler(
        ILogger<GetImagesHandler> logger,
        IValidator<GetImagesQuery> validator,
        HybridCache hybridCache,
        IReadDbContext readDbContext)
    {
        _logger = logger;
        _validator = validator;
        _hybridCache = hybridCache;
        _readDbContext = readDbContext;
    }

    public async Task<Result<PagedList<Image>>> Handle(
        GetImagesQuery query, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrorList();
        
        var imagesCahce = await _hybridCache.GetOrCreateAsync(
            $"images_{query.UserId}", 
            async _ =>
            {
                var images = await _readDbContext.Images
                    .Where(i => i.UserId == query.UserId)
                    .ToListAsync(cancellationToken);
                
                return images;
            },
            cancellationToken: cancellationToken);

        if (imagesCahce.Count == 0)
            return new PagedList<Image>();

        var images = imagesCahce.AsQueryable();
        
        images = ImagesQueryFilter(query, images);

        var keySelector = SortByProperty(query);

        images = query.SortDirection?.ToLower() == "desc"
            ? images.OrderByDescending(keySelector) 
            : images.OrderBy(keySelector);
        
        var pagedList = images.ToList().ToPagedList(
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
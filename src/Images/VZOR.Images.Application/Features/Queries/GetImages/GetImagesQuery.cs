using VZOR.Core.Abstractions;

namespace VZOR.Images.Application.Features.Queries.GetImages;

public record GetImagesQuery(
    Guid UserId,
    DateTime? StartUploadDate,
    DateTime? EndUploadDate,
    string? SortBy,
    string? SortDirection,
    int Page,
    int PageSize) : IQuery;

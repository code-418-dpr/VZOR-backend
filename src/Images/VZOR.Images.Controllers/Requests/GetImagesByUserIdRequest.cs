namespace VZOR.Images.Controllers.Requests;

public record GetImagesByUserIdRequest(
    DateTime? StartUploadDate,
    DateTime? EndUploadDate,
    string? SortBy,
    string? SortDirection,
    int Page,
    int PageSize);

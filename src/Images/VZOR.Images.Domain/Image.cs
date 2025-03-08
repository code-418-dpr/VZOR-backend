using VZOR.SharedKernel;
using VZOR.SharedKernel.Errors;

namespace VZOR.Images.Domain;

public class Image
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required DateTime UploadDate { get; init; }
    public required string UploadLink { get; init; }
    public string? ProcessingResult { get; set; }
}
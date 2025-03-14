namespace VZOR.Core.Dtos;

public class ImageDto
{
    public required string Id { get; init; }
    public required string UserId { get; init; }
    public required DateTime UploadDate { get; init; }
    public required string UploadLink { get; init; }
    public string? ProcessingResult { get; set; }
    public string PresignedDownloadUrl { get; set; }
}
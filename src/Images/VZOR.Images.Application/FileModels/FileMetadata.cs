namespace VZOR.Images.Application.FileModels;

public record FileMetadata(string BucketName, string ObjectName);


public class FileMetadataS3
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string UploadId { get; set; } = string.Empty;
}
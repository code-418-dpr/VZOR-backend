namespace VZOR.Images.Application.FileProvider;

public record FileData(Stream Stream, FileInfo FileInfo);

public record FileInfo(string FilePath, string BucketName);
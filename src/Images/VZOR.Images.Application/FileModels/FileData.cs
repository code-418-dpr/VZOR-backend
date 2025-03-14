namespace VZOR.Images.Application.FileModels;

public record FileData(Stream Stream, FileInfo FileInfo);

public record FileInfo(string FilePath, string BucketName);
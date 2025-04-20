namespace VZOR.Images.Contracts.Responses;

public record UploadImageUrl(string Url);

public record UploadImageInS3Response(IEnumerable<UploadImageUrl> Urls);

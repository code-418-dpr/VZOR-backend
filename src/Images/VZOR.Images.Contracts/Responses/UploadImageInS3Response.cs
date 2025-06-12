namespace VZOR.Images.Contracts.Responses;

public record UploadImageUrl(Guid FileId, string Url);

public record UploadImageInS3Response(IEnumerable<UploadImageUrl> Urls);

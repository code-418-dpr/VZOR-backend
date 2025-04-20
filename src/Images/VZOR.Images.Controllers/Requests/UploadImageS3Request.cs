using VZOR.Images.Application.FileModels;

namespace VZOR.Images.Controllers.Requests;

public record UploadImageS3Request(IEnumerable<FileDataS3> Files);

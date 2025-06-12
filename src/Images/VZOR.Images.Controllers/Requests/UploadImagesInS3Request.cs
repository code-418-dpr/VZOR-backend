using VZOR.Images.Application.FileModels;

namespace VZOR.Images.Controllers.Requests;

public record UploadImagesInS3Request(IEnumerable<FileDataS3> Files);

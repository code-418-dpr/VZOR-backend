using Microsoft.AspNetCore.Http;

namespace VZOR.Images.Controllers.Requests;

public record UploadImagesRequest(IFormFileCollection Files);

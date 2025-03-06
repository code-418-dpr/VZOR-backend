using Microsoft.AspNetCore.Http;

namespace VZOR.Images.Controllers.Requests;

public record AddVisitorPhotosRequest(IFormFileCollection Files);

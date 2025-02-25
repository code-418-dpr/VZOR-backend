using Microsoft.AspNetCore.Http;

namespace WhoCame.Visitors.Controllers.Requests;

public record AddVisitorPhotosRequest(IFormFileCollection Files);

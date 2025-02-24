using Microsoft.AspNetCore.Http;

namespace WhoCame.Visitors.Contracts.Requests;

public record AddVisitorPhotosRequest(IFormFileCollection Files);

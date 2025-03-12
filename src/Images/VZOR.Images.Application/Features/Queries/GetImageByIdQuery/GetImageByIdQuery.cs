using VZOR.Core.Abstractions;

namespace VZOR.Images.Application.Features.Queries.GetImageByIdQuery;

public record GetImageByIdQuery(Guid ImageId) : IQuery;

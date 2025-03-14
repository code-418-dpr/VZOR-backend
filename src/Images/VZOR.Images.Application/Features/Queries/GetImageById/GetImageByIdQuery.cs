using VZOR.Core.Abstractions;

namespace VZOR.Images.Application.Features.Queries.GetImageById;

public record GetImageByIdQuery(Guid ImageId) : IQuery;

using VZOR.Core.Abstractions;

namespace VZOR.Images.Application.Features.Queries.GetImagesByQuery;

public record GetImagesByQueryQuery(Guid UserId, string Query) : IQuery;

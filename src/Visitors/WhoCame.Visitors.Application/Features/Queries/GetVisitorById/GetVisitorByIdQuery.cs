using WhoCame.Core.Abstractions;

namespace WhoCame.Visitors.Application.Features.Queries.GetVisitorById;

public record GetVisitorByIdQuery(Guid VisitorId) : IQuery;

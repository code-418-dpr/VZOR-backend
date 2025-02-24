using WhoCame.Core.Abstractions;

namespace WhoCame.Visitors.Application.Features.Commands.DeleteVisitor;

public record DeleteVisitorCommand(Guid Id) : ICommand;

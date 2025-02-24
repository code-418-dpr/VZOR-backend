using WhoCame.Core.Abstractions;

namespace WhoCame.Visitors.Application.Features.Commands.AddVisitor;

public record AddVisitorCommand(string FirstName, string LastName, string? MiddleName): ICommand;

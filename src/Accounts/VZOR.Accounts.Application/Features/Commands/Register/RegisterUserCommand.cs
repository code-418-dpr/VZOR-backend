using VZOR.Core.Abstractions;

namespace VZOR.Accounts.Application.Features.Commands.Register;

public record RegisterUserCommand(
    string Name,
    string Email,
    string Password): ICommand;
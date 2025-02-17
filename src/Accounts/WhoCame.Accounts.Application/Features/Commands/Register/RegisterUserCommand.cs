using WhoCame.Core.Abstractions;

namespace WhoCame.Accounts.Application.Features.Commands.Register;

public record RegisterUserCommand(
    string Email,
    string Password): ICommand;
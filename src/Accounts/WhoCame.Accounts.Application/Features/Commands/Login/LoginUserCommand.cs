using WhoCame.Core.Abstractions;

namespace WhoCame.Accounts.Application.Features.Commands.Login;

public record LoginUserCommand(string Email, string Password): ICommand;
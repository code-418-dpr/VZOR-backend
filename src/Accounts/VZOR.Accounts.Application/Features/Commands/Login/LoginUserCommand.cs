using VZOR.Core.Abstractions;

namespace VZOR.Accounts.Application.Features.Commands.Login;

public record LoginUserCommand(string Email, string Password): ICommand;
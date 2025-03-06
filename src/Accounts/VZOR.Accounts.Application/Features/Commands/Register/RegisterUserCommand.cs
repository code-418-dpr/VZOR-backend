using VZOR.Core.Abstractions;

namespace VZOR.Accounts.Application.Features.Commands.Register;

public record RegisterUserCommand(
    string Email,
    string Password): ICommand;
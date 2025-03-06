using VZOR.Core.Abstractions;

namespace VZOR.Accounts.Application.Features.Commands.Register;

public record RegisterUserCommand(
    string FirstName,
    string LastName,
    string? MiddleName,
    string Email,
    string Password): ICommand;
using VZOR.Core.Abstractions;

namespace VZOR.Accounts.Application.Features.Commands.ChangeUserData;

public record ChangeUserDataCommand(Guid UserId, string? Name,string? CurrentPassword, string? NewPassword) : ICommand;

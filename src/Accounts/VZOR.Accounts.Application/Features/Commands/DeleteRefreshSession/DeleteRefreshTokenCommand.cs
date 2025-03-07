using VZOR.Core.Abstractions;

namespace VZOR.Accounts.Application.Features.Commands.DeleteRefreshSession;

public record DeleteRefreshTokenCommand(Guid RefreshToken) : ICommand;

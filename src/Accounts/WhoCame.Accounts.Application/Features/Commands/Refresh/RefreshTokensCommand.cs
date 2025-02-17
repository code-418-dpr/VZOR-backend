using WhoCame.Core.Abstractions;

namespace WhoCame.Accounts.Application.Features.Commands.Refresh;

public record RefreshTokensCommand(string AccessToken, Guid RefreshToken) : ICommand;
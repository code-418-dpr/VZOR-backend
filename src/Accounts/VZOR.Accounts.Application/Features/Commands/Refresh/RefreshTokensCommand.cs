using VZOR.Core.Abstractions;

namespace VZOR.Accounts.Application.Features.Commands.Refresh;

public record RefreshTokensCommand(string AccessToken, Guid RefreshToken) : ICommand;
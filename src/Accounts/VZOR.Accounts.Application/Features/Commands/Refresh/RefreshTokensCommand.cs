using VZOR.Core.Abstractions;

namespace VZOR.Accounts.Application.Features.Commands.Refresh;

public record RefreshTokensCommand(Guid RefreshToken) : ICommand;
using WhoCame.Accounts.Domain;
using WhoCame.SharedKernel;

namespace WhoCame.Accounts.Application.Managers;

public interface IRefreshSessionManager
{
    void Delete(RefreshSession refreshSession);
    Task<Result<RefreshSession>> GetByRefreshToken(Guid refreshToken, CancellationToken cancellationToken = default);
}
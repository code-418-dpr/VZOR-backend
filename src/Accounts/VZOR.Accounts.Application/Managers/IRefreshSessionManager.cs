using VZOR.Accounts.Domain;
using VZOR.SharedKernel;

namespace VZOR.Accounts.Application.Managers;

public interface IRefreshSessionManager
{
    void Delete(RefreshSession refreshSession);
    Task<Result<RefreshSession>> GetByRefreshToken(Guid refreshToken, CancellationToken cancellationToken = default);
}
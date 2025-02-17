using WhoCame.SharedKernel;

namespace WhoCame.Accounts.Application.Managers;

public interface IPermissionManager
{
    Task<Result<List<string>>> GetPermissionsByUserId(Guid userId, CancellationToken cancellationToken = default);
}
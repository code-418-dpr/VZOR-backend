using Microsoft.EntityFrameworkCore;
using VZOR.Accounts.Application.Managers;
using VZOR.Accounts.Domain;
using VZOR.SharedKernel;
using VZOR.SharedKernel.Errors;

namespace VZOR.Accounts.Infrastructure.IdentityManagers;

public class RefreshSessionManager(AccountsDbContext accountsDbContext) : IRefreshSessionManager
{

    public async Task<Result<RefreshSession>> GetByRefreshToken(
        Guid refreshToken, CancellationToken cancellationToken = default)
    {
        var refreshSession = await accountsDbContext.RefreshSessions
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.RefreshToken == refreshToken, cancellationToken);

        if (refreshSession is null)
            return Errors.General.NotFound();

        return refreshSession;
    }
    
    public void Delete(RefreshSession refreshSession)
    {
        accountsDbContext.RefreshSessions.Remove(refreshSession);
    }
}
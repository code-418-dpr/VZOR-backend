using Microsoft.EntityFrameworkCore;
using WhoCame.Accounts.Application.Managers;
using WhoCame.Accounts.Domain;
using WhoCame.SharedKernel;
using WhoCame.SharedKernel.Errors;

namespace WhoCame.Accounts.Infrastructure.IdentityManagers;

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
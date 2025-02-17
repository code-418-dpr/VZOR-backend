using Microsoft.EntityFrameworkCore;
using WhoCame.Accounts.Application.Managers;
using WhoCame.Accounts.Domain;
using WhoCame.SharedKernel;
using WhoCame.SharedKernel.Errors;

namespace WhoCame.Accounts.Infrastructure.IdentityManagers;

public class PermissionManager(AccountsDbContext accountsDbContext) : IPermissionManager
{
    public async Task<Permission?> FindByCode(string code)
    {
        return await accountsDbContext.Permissions.FirstOrDefaultAsync(p => p.Code == code);
    }
    
    public async Task AddRangeIfExist(IEnumerable<string> permissions)
    {
        foreach (var permissionCode in permissions)
        {
            var isPermissionExist = await accountsDbContext.Permissions
                .AnyAsync(p => p.Code == permissionCode);
        
            if(isPermissionExist)
                continue;

            await accountsDbContext.Permissions.AddAsync(new Permission { Code = permissionCode });
        }

        await accountsDbContext.SaveChangesAsync();
    }

    public async Task<Result<List<string>>> GetPermissionsByUserId(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await accountsDbContext.Users
            .Include(u => u.Roles)
                .ThenInclude(r => r.RolePermissions)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        
        if (user is null)
            return Errors.General.NotFound();

        var permissions = user.Roles
            .SelectMany(r => r.RolePermissions.Select(rp => rp.Permission.Code)).ToList();
        
        return permissions;
    }
}
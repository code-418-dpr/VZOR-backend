﻿using Microsoft.EntityFrameworkCore;
using WhoCame.Accounts.Domain;

namespace WhoCame.Accounts.Infrastructure.IdentityManagers;

public class  RolePermissionManager(AccountsDbContext accountsDbContext)
{
    public async Task AddRangeIfExist(Guid roleId,IEnumerable<string> permissions)
    {
        foreach (var permissionCode in permissions)
        {
            var permission = await accountsDbContext.Permissions
                .FirstOrDefaultAsync(p => p.Code == permissionCode);

            if (permission is null)
                throw new ApplicationException("permission is cannot be null");

            var rolePermissionExist = await accountsDbContext.RolePermissions
                .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permission!.Id);
                
            if(rolePermissionExist)
                continue;

            await accountsDbContext.RolePermissions.AddAsync(
                new RolePermission { RoleId = roleId, PermissionId = permission!.Id });
        }

        await accountsDbContext.SaveChangesAsync();
    }
}
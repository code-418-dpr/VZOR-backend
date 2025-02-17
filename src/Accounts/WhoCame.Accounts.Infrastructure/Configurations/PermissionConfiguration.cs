using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhoCame.Accounts.Domain;

namespace WhoCame.Accounts.Infrastructure.Configurations;

public class PermissionConfiguration: IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.HasIndex(p => p.Code)
            .IsUnique();
    }
}
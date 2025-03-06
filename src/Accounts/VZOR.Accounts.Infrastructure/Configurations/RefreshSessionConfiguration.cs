using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VZOR.Accounts.Domain;

namespace VZOR.Accounts.Infrastructure.Configurations;

public class RefreshSessionConfiguration: IEntityTypeConfiguration<RefreshSession>
{
    public void Configure(EntityTypeBuilder<RefreshSession> builder)
    {
        builder.ToTable("refresh_sessions");

        builder.HasOne(rs => rs.User)
            .WithMany()
            .HasForeignKey(rs => rs.UserId);
    }
}
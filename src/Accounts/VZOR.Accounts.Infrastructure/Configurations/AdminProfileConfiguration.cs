using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VZOR.Accounts.Domain;

namespace VZOR.Accounts.Infrastructure.Configurations;

public class AdminProfileConfiguration: IEntityTypeConfiguration<AdminProfile>
{
    public void Configure(EntityTypeBuilder<AdminProfile> builder)
    {
        builder.ToTable("admin_profiles");
        
        builder.HasOne(ap => ap.User)
            .WithOne()
            .HasForeignKey<AdminProfile>(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasKey(pa => pa.Id);
        
    }
}
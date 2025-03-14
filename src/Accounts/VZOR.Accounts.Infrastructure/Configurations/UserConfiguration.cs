using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VZOR.Accounts.Domain;

namespace VZOR.Accounts.Infrastructure.Configurations;

public class UserConfiguration: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        
        builder.HasMany(u => u.Roles)
            .WithMany(r => r.Users);
        
        builder.Property(u => u.IsActive)
            .HasDefaultValue(true)
            .IsRequired()
            .HasColumnName("is_active");

        builder.Navigation(u => u.ParticipantAccount)
            .AutoInclude();
        
        builder.Navigation(u => u.AdminProfile)
            .AutoInclude();
    }
}
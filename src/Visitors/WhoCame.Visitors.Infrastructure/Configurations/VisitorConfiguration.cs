using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhoCame.SharedKernel.Constraints;
using WhoCame.Visitors.Domain;

namespace WhoCame.Visitors.Infrastructure.Configurations;

public class VisitorConfiguration: IEntityTypeConfiguration<Visitor>
{
    public void Configure(EntityTypeBuilder<Visitor> builder)
    {
        builder.ToTable("visitors");
        
        builder.HasKey(v => v.Id)
            .HasName("id");

        builder.Property(v => v.FirstName)
            .HasMaxLength(Constraints.MAX_VALUE_LENGTH)
            .HasColumnName("first_name")
            .IsRequired();
        
        builder.Property(v => v.LastName)
            .HasMaxLength(Constraints.MAX_VALUE_LENGTH)
            .HasColumnName("last_name")
            .IsRequired();
        
        builder.Property(v => v.MiddleName)
            .HasMaxLength(Constraints.MAX_VALUE_LENGTH)
            .HasColumnName("middle_name")
            .IsRequired(false);
        
        builder.Property(p => p.VisitorPhotos)
            .HasConversion(
                photos => JsonSerializer.Serialize(photos, JsonSerializerOptions.Default),
                jsonPhoto => JsonSerializer.Deserialize<List<string>>(jsonPhoto, JsonSerializerOptions.Default)!)
            .HasColumnType("jsonb")
            .HasColumnName("visitor_photos");
    }
}
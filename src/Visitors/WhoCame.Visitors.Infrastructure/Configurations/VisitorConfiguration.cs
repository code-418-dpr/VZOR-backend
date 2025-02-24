using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WhoCame.SharedKernel.Constraints;
using WhoCame.Visitors.Domain;

namespace WhoCame.Visitors.Infrastructure.Configurations;

public class StringListJsonConverter : ValueConverter<List<string>, string>
{
    public StringListJsonConverter() : base(
        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!)!)
    {
    }
}

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
        
        builder.Property(v => v.VisitorPhotos)
            .HasColumnName("visitor_photos")
            .HasColumnType("jsonb")
            .HasConversion(new StringListJsonConverter());
    }
}
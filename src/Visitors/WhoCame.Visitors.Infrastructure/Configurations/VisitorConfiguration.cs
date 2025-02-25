using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using WhoCame.SharedKernel.Constraints;
using WhoCame.Visitors.Domain;

namespace WhoCame.Visitors.Infrastructure.Configurations;

public class StringListJsonConverter : ValueConverter<List<string>, string>
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        ObjectCreationHandling = ObjectCreationHandling.Replace,
        DefaultValueHandling = DefaultValueHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.Indented
    };

    public StringListJsonConverter() : base(
        v => JsonConvert.SerializeObject(v, JsonSerializerSettings),
        v => JsonConvert.DeserializeObject<List<string>>(v, JsonSerializerSettings)!)
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
            .HasColumnName("visitor_photos");
    }
}
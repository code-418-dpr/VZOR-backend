using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VZOR.Images.Domain;

namespace VZOR.Images.Infrastructure.Configurations;

public class ImageConfiguration: IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.ToTable("images");
        
        builder.HasKey(i => i.Id);
        
        builder.Property(i => i.UserId)
            .HasColumnName("user_id")
            .IsRequired();
        
        builder.Property(i => i.UploadLink)
            .HasColumnName("upload_link")
            .IsRequired();

        builder.Property(i => i.UploadDate)
            .HasColumnName("upload_date")
            .IsRequired();

        builder.Property(i => i.ProcessingResult)
            .HasColumnName("processing_result")
            .HasColumnType("jsonb")
            .IsRequired(false);
    }
}
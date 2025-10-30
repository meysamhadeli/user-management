using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagement.Industries.Models;

namespace UserManagement.Data.Configurations;

public class IndustryConfiguration : IEntityTypeConfiguration<Industry>
{
    public void Configure(EntityTypeBuilder<Industry> builder)
    {
        builder.ToTable(nameof(Industry));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.HasMany(x => x.Companies)
            .WithOne(x => x.Industry)
            .HasForeignKey(x => x.IndustryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Version)
            .IsConcurrencyToken();
    }
}
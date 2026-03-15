using CafeManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeManager.Infrastructure.Data.Configurations;

public class CafeConfiguration : IEntityTypeConfiguration<Cafe>
{
    public void Configure(EntityTypeBuilder<Cafe> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(c => c.Location)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Logo)
            .HasMaxLength(512); // Stores file path or base64 URL

        // One café has many CafeEmployee records;
        // deleting a café cascades to its CafeEmployee records AND employees.
        builder.HasMany(c => c.CafeEmployees)
            .WithOne(ce => ce.Cafe)
            .HasForeignKey(ce => ce.CafeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

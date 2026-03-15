using CafeManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeManager.Infrastructure.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasMaxLength(9); // UIXXXXXXX = 9 chars

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(e => e.EmailAddress)
            .IsRequired()
            .HasMaxLength(254);

        builder.Property(e => e.PhoneNumber)
            .IsRequired()
            .HasMaxLength(8);

        builder.Property(e => e.Gender)
            .IsRequired()
            .HasConversion<string>(); // Store as "Male"/"Female" string in DB

        builder.HasIndex(e => e.EmailAddress).IsUnique();

        // One-to-one: each employee can only have one CafeEmployee record
        builder.HasOne(e => e.CafeEmployee)
            .WithOne(ce => ce.Employee)
            .HasForeignKey<CafeEmployee>(ce => ce.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

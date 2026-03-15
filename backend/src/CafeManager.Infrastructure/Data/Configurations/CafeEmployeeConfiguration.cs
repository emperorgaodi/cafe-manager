using CafeManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeManager.Infrastructure.Data.Configurations;

public class CafeEmployeeConfiguration : IEntityTypeConfiguration<CafeEmployee>
{
    public void Configure(EntityTypeBuilder<CafeEmployee> builder)
    {
        // Composite primary key — an employee can only appear once
        builder.HasKey(ce => new { ce.CafeId, ce.EmployeeId });

        builder.Property(ce => ce.StartDate)
            .IsRequired();
    }
}

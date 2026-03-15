using CafeManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CafeManager.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Cafe> Cafes => Set<Cafe>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<CafeEmployee> CafeEmployees => Set<CafeEmployee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all IEntityTypeConfiguration classes in this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

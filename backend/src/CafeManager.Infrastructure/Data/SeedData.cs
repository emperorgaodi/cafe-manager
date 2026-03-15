using CafeManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CafeManager.Infrastructure.Data;

/// <summary>
/// Seeds initial cafés and employees into the database.
/// Only runs if the tables are empty to avoid duplicate data on restarts.
/// </summary>
public static class SeedData
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.Cafes.AnyAsync())
            return; // Already seeded

        // ── Cafés ──────────────────────────────────────────────────────────────

        var cafeAbc = Cafe.Create("ABC Cafe", "A cozy corner café with great coffee.", "Marina Bay");
        var cafeDef = Cafe.Create("DEF Bistro", "Modern bistro with artisan sandwiches.", "Orchard Road");
        var cafeGhi = Cafe.Create("GHI Corner", "Family-run café with homemade pastries.", "Tampines");

        context.Cafes.AddRange(cafeAbc, cafeDef, cafeGhi);
        await context.SaveChangesAsync();

        // ── Employees ─────────────────────────────────────────────────────────

        var alice = Employee.Create("Alice Tan", "alice.tan@email.com", "91234567", Gender.Female);
        var bob = Employee.Create("Bob Lee", "bob.lee@email.com", "81234567", Gender.Male);
        var charlie = Employee.Create("Charlie Ng", "charlie.ng@email.com", "92345678", Gender.Male);
        var diana = Employee.Create("Diana Koh", "diana.koh@email.com", "82345678", Gender.Female);
        var evan = Employee.Create("Evan Lim", "evan.lim@email.com", "93456789", Gender.Male);

        context.Employees.AddRange(alice, bob, charlie, diana, evan);
        await context.SaveChangesAsync();

        // ── Assignments ───────────────────────────────────────────────────────

        context.CafeEmployees.AddRange(
            CafeEmployee.Create(cafeAbc.Id, alice.Id, new DateOnly(2023, 1, 15)),
            CafeEmployee.Create(cafeAbc.Id, bob.Id, new DateOnly(2023, 6, 1)),
            CafeEmployee.Create(cafeDef.Id, charlie.Id, new DateOnly(2022, 9, 20)),
            CafeEmployee.Create(cafeDef.Id, diana.Id, new DateOnly(2024, 2, 10)),
            CafeEmployee.Create(cafeGhi.Id, evan.Id, new DateOnly(2023, 11, 5))
        );

        await context.SaveChangesAsync();
    }
}

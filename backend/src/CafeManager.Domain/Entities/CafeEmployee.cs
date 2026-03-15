namespace CafeManager.Domain.Entities;

/// <summary>
/// Represents the relationship between an employee and a café,
/// including the employee's start date at that café.
/// An employee can only belong to one café at a time (enforced at DB level).
/// </summary>
public class CafeEmployee
{
    public Guid CafeId { get; private set; }
    public string EmployeeId { get; private set; } = string.Empty;
    public DateOnly StartDate { get; private set; }

    // Navigation properties
    public Cafe Cafe { get; private set; } = null!;
    public Employee Employee { get; private set; } = null!;

    private CafeEmployee() { } // Required by EF Core

    public static CafeEmployee Create(Guid cafeId, string employeeId, DateOnly startDate)
    {
        return new CafeEmployee
        {
            CafeId = cafeId,
            EmployeeId = employeeId,
            StartDate = startDate
        };
    }

    /// <summary>
    /// Calculates how many days the employee has worked at this café.
    /// </summary>
    public int DaysWorked => DateOnly.FromDateTime(DateTime.UtcNow).DayNumber - StartDate.DayNumber;
}

using System.Security.Cryptography;

namespace CafeManager.Domain.Entities;

public class Employee
{
    public string Id { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string EmailAddress { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public Gender Gender { get; private set; }

    public CafeEmployee? CafeEmployee { get; private set; }

    private Employee() { } // Required by EF Core

    public static Employee Create(string name, string emailAddress, string phoneNumber, Gender gender)
    {
        return new Employee
        {
            Id = GenerateEmployeeId(),
            Name = name,
            EmailAddress = emailAddress,
            PhoneNumber = phoneNumber,
            Gender = gender
        };
    }

    public void Update(string name, string emailAddress, string phoneNumber, Gender gender)
    {
        Name = name;
        EmailAddress = emailAddress;
        PhoneNumber = phoneNumber;
        Gender = gender;
    }

    /// <summary>
    /// Assigns this employee to a café. Preserves the start date if the café hasn't changed.
    /// Resets the start date to today when moving to a different café.
    /// </summary>
    public void AssignToCafe(Guid cafeId)
    {
        if (CafeEmployee?.CafeId == cafeId) return;
        CafeEmployee = CafeEmployee.Create(cafeId, Id, DateOnly.FromDateTime(DateTime.UtcNow));
    }

    /// <summary>Removes the employee's café assignment.</summary>
    public void UnassignFromCafe()
    {
        CafeEmployee = null;
    }

    /// <summary>
    /// Generates a cryptographically random employee ID in the format 'UIXXXXXXX'.
    /// Uses <see cref="RandomNumberGenerator"/> instead of <see cref="Random"/> to
    /// avoid thread-safety issues and prevent predictable ID sequences in production.
    /// </summary>
    private static string GenerateEmployeeId()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        // RandomNumberGenerator is thread-safe and cryptographically random
        var randomBytes = RandomNumberGenerator.GetBytes(7);
        var suffix = new string(randomBytes.Select(b => chars[b % chars.Length]).ToArray());
        return $"UI{suffix}";
    }
}

public enum Gender
{
    Male,
    Female
}

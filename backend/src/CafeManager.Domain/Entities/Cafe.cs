namespace CafeManager.Domain.Entities;

public class Cafe
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? Logo { get; private set; }
    public string Location { get; private set; } = string.Empty;

    // Navigation property
    public ICollection<CafeEmployee> CafeEmployees { get; private set; } = new List<CafeEmployee>();

    private Cafe() { } // Required by EF Core

    public static Cafe Create(string name, string description, string location, string? logo = null)
    {
        return new Cafe
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Location = location,
            Logo = logo
        };
    }

    public void Update(string name, string description, string location, string? logo)
    {
        Name = name;
        Description = description;
        Location = location;
        Logo = logo;
    }
}

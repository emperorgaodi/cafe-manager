using MediatR;

namespace CafeManager.Application.Cafes.Queries.GetCafes;

/// <summary>
/// Query to retrieve all cafés, optionally filtered by location.
/// Results are sorted by employee count (highest first).
/// </summary>
public record GetCafesQuery(string? Location) : IRequest<IEnumerable<CafeDto>>;

public record CafeDto(
    Guid Id,
    string Name,
    string Description,
    int Employees,
    string? Logo,
    string Location
);

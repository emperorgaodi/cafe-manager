using CafeManager.Domain.Entities;
using CafeManager.Domain.Repositories;
using CafeManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CafeManager.Infrastructure.Repositories;

public class CafeRepository : ICafeRepository
{
    private readonly AppDbContext _context;

    public CafeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Cafe>> GetAllAsync(string? location = null, CancellationToken ct = default)
    {
        var query = _context.Cafes.Include(c => c.CafeEmployees).AsQueryable();

        if (!string.IsNullOrWhiteSpace(location))
        {
            query = query.Where(c => EF.Functions.ILike(c.Location, $"%{location}%"));
        }

        return await query.ToListAsync(ct);
    }

    public async Task<Cafe?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Cafes
            .Include(c => c.CafeEmployees)
                .ThenInclude(ce => ce.Employee)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await _context.Cafes.AnyAsync(c => c.Id == id, ct);

    public async Task AddAsync(Cafe cafe, CancellationToken ct = default)
        => await _context.Cafes.AddAsync(cafe, ct);

    public void Update(Cafe cafe)
        => _context.Cafes.Update(cafe);

    public void Delete(Cafe cafe)
        => _context.Cafes.Remove(cafe);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
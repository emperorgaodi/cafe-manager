using CafeManager.Domain.Entities;
using CafeManager.Domain.Repositories;
using CafeManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CafeManager.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Employee>> GetAllAsync(Guid? cafeId = null, CancellationToken ct = default)
    {
        var query = _context.Employees
            .Include(e => e.CafeEmployee)
            .ThenInclude(ce => ce!.Cafe)
            .AsQueryable();

        if (cafeId.HasValue)
            query = query.Where(e => e.CafeEmployee != null && e.CafeEmployee.CafeId == cafeId.Value);

        return await query.ToListAsync(ct);
    }

    public async Task<Employee?> GetByIdAsync(string id, CancellationToken ct = default)
        => await _context.Employees
            .Include(e => e.CafeEmployee)
            .ThenInclude(ce => ce!.Cafe)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<bool> ExistsAsync(string id, CancellationToken ct = default)
        => await _context.Employees.AnyAsync(e => e.Id == id, ct);

    public async Task AddAsync(Employee employee, CancellationToken ct = default)
        => await _context.Employees.AddAsync(employee, ct);

    public void Update(Employee employee)
        => _context.Employees.Update(employee);

    public void Delete(Employee employee)
        => _context.Employees.Remove(employee);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}

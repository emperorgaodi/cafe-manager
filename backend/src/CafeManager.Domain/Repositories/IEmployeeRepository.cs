using CafeManager.Domain.Entities;

namespace CafeManager.Domain.Repositories;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllAsync(Guid? cafeId = null, CancellationToken ct = default);
    Task<Employee?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<bool> ExistsAsync(string id, CancellationToken ct = default);
    Task AddAsync(Employee employee, CancellationToken ct = default);
    void Update(Employee employee);
    void Delete(Employee employee);
    Task SaveChangesAsync(CancellationToken ct = default);
}

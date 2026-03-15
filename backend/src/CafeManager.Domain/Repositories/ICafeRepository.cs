using CafeManager.Domain.Entities;

namespace CafeManager.Domain.Repositories;

public interface ICafeRepository
{
    Task<IEnumerable<Cafe>> GetAllAsync(string? location = null, CancellationToken ct = default);
    Task<Cafe?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Cafe cafe, CancellationToken ct = default);
    void Update(Cafe cafe);
    void Delete(Cafe cafe);
    Task SaveChangesAsync(CancellationToken ct = default);
}

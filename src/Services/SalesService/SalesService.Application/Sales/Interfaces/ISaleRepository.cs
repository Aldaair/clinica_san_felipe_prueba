using SalesService.Domain.Entities;

namespace SalesService.Application.Sales.Interfaces;

public interface ISaleRepository
{
    Task AddAsync(SaleCab sale, CancellationToken cancellationToken = default);
    Task<SaleCab?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SaleCab>> GetAllAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
using SalesService.Domain.Entities;

namespace SalesService.Application.Sales.Interfaces;

public interface IProductPricingSnapshotRepository
{
    Task<IReadOnlyList<ProductPricingSnapshot>> GetByIdsAsync(
        IEnumerable<int> productIds,
        CancellationToken cancellationToken = default);

    Task<ProductPricingSnapshot?> GetByIdAsync(
        int productId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        ProductPricingSnapshot snapshot,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
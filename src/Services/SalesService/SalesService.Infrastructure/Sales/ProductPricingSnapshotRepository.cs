using Microsoft.EntityFrameworkCore;
using SalesService.Application.Sales.Interfaces;
using SalesService.Domain.Entities;
using SalesService.Infrastructure.Persistence;

namespace SalesService.Infrastructure.Sales;

public sealed class ProductPricingSnapshotRepository : IProductPricingSnapshotRepository
{
    private readonly SalesDbContext _dbContext;

    public ProductPricingSnapshotRepository(SalesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ProductPricingSnapshot>> GetByIdsAsync(
        IEnumerable<int> productIds,
        CancellationToken cancellationToken = default)
    {
        var ids = productIds.Distinct().ToList();

        return await _dbContext.ProductPricingSnapshots
            .Where(x => ids.Contains(x.IdProducto))
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductPricingSnapshot?> GetByIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductPricingSnapshots
            .FirstOrDefaultAsync(x => x.IdProducto == productId, cancellationToken);
    }

    public async Task AddAsync(
        ProductPricingSnapshot snapshot,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.ProductPricingSnapshots.AddAsync(snapshot, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
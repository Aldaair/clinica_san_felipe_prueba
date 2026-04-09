using Microsoft.EntityFrameworkCore;
using ProductService.Application.Products.Interfaces;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Infrastructure.Products;

public sealed class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _dbContext;

    public ProductRepository(ProductDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _dbContext.Products.AddAsync(product, cancellationToken);
    }
    public async Task AddOutboxMessageAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        await _dbContext.OutboxMessages.AddAsync(message, cancellationToken);
    }
    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await action(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
    public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.Distinct().ToList();

        return await _dbContext.Products
            .Where(x => idList.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAndLotAsync(string nombreProducto, string nroLote, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products.AnyAsync(
            x => x.NombreProducto == nombreProducto && x.NroLote == nroLote,
            cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsPriceUpdateLogAsync(int purchaseId, int productId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductPriceUpdateLogs
            .AnyAsync(x => x.PurchaseId == purchaseId && x.ProductId == productId, cancellationToken);
    }

    public async Task AddPriceUpdateLogAsync(ProductPriceUpdateLog log, CancellationToken cancellationToken = default)
    {
        await _dbContext.ProductPriceUpdateLogs.AddAsync(log, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductPriceUpdateLog>> GetPriceUpdateLogsByPurchaseIdAsync(int purchaseId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductPriceUpdateLogs
            .Where(x => x.PurchaseId == purchaseId)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }
}
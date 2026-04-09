using ProductService.Domain.Entities;

namespace ProductService.Application.Products.Interfaces;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAndLotAsync(string nombreProducto, string nroLote, CancellationToken cancellationToken = default);
    Task AddOutboxMessageAsync(OutboxMessage message, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);

    Task<bool> ExistsPriceUpdateLogAsync(int purchaseId, int productId, CancellationToken cancellationToken = default);

    Task AddPriceUpdateLogAsync(ProductPriceUpdateLog log, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductPriceUpdateLog>> GetPriceUpdateLogsByPurchaseIdAsync(int purchaseId, CancellationToken cancellationToken = default);
}
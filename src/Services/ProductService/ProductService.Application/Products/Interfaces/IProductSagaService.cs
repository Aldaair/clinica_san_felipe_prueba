using ProductService.Application.Products.DTOs.Saga;

namespace ProductService.Application.Products.Interfaces;

public interface IProductSagaService
{
    Task<ProductPurchaseBatchUpdateResponse> ApplyPurchaseBatchUpdateAsync(
        ProductPurchaseBatchUpdateRequest request,
        CancellationToken cancellationToken = default);

    Task<ProductPurchaseBatchUpdateResponse> RollbackPurchaseBatchUpdateAsync(
        ProductPurchaseBatchRollbackRequest request,
        CancellationToken cancellationToken = default);
}
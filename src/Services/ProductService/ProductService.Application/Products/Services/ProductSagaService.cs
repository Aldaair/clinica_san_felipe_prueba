using ProductService.Application.Products.DTOs.Saga;
using ProductService.Application.Products.Interfaces;
using ProductService.Domain.Entities;

namespace ProductService.Application.Products.Services;

public sealed class ProductSagaService : IProductSagaService
{
    private readonly IProductRepository _productRepository;

    public ProductSagaService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductPurchaseBatchUpdateResponse> ApplyPurchaseBatchUpdateAsync(
        ProductPurchaseBatchUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Items is null || request.Items.Count == 0)
            throw new InvalidOperationException("No se enviaron productos para actualizar.");

        var ids = request.Items.Select(x => x.IdProducto).Distinct().ToList();
        var products = await _productRepository.GetByIdsAsync(ids, cancellationToken);
        var productsById = products.ToDictionary(x => x.Id);

        var missingIds = ids.Where(x => !productsById.ContainsKey(x)).ToList();
        if (missingIds.Count > 0)
            throw new InvalidOperationException($"No existen los productos: {string.Join(", ", missingIds)}");

        int updated = 0;

        foreach (var item in request.Items)
        {
            var product = productsById[item.IdProducto];

            var logExists = await _productRepository.ExistsPriceUpdateLogAsync(
                request.PurchaseId,
                item.IdProducto,
                cancellationToken);

            // Idempotencia: si ya se aplicó para esa compra y producto, lo saltamos
            if (logExists)
                continue;

            var oldCosto = product.Costo;
            var oldPrecioVenta = product.PrecioVenta;

            product.ApplyPurchaseUpdate(item.NuevoCosto);

            var log = new ProductPriceUpdateLog(
                request.PurchaseId,
                item.IdProducto,
                oldCosto,
                oldPrecioVenta,
                product.Costo,
                product.PrecioVenta
            );

            await _productRepository.AddPriceUpdateLogAsync(log, cancellationToken);
            updated++;
        }

        await _productRepository.SaveChangesAsync(cancellationToken);

        return new ProductPurchaseBatchUpdateResponse(
            request.PurchaseId,
            updated,
            "Actualización de productos aplicada correctamente."
        );
    }

    public async Task<ProductPurchaseBatchUpdateResponse> RollbackPurchaseBatchUpdateAsync(
        ProductPurchaseBatchRollbackRequest request,
        CancellationToken cancellationToken = default)
    {
        var logs = await _productRepository.GetPriceUpdateLogsByPurchaseIdAsync(request.PurchaseId, cancellationToken);

        if (logs.Count == 0)
        {
            return new ProductPurchaseBatchUpdateResponse(
                request.PurchaseId,
                0,
                "No existen logs para rollback."
            );
        }

        var productIds = logs.Select(x => x.ProductId).Distinct().ToList();
        var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);
        var productsById = products.ToDictionary(x => x.Id);

        int restored = 0;

        foreach (var log in logs.Where(x => !x.RolledBack))
        {
            if (!productsById.TryGetValue(log.ProductId, out var product))
                continue;

            product.RestorePrice(log.OldCosto, log.OldPrecioVenta);
            log.MarkRolledBack();
            restored++;
        }

        await _productRepository.SaveChangesAsync(cancellationToken);

        return new ProductPurchaseBatchUpdateResponse(
            request.PurchaseId,
            restored,
            "Rollback de productos aplicado correctamente."
        );
    }
}
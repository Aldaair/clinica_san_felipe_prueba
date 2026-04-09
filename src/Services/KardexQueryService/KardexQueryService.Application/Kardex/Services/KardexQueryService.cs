using KardexQueryService.Application.Kardex.DTOs;
using KardexQueryService.Application.Kardex.Interfaces;

namespace KardexQueryService.Application.Kardex.Services;

public sealed class KardexQueryService : IKardexQueryService
{
    private readonly IProductQueryGateway _productGateway;
    private readonly IMovementQueryGateway _movementGateway;

    public KardexQueryService(
        IProductQueryGateway productGateway,
        IMovementQueryGateway movementGateway)
    {
        _productGateway = productGateway;
        _movementGateway = movementGateway;
    }

    public async Task<IReadOnlyList<KardexResponse>> GetKardexAsync(CancellationToken cancellationToken = default)
    {
        var productsTask = _productGateway.GetProductsAsync(cancellationToken);
        var stockTask = _movementGateway.GetCurrentStockAsync(cancellationToken);

        await Task.WhenAll(productsTask, stockTask);

        var products = productsTask.Result.ToDictionary(x => x.Id);
        var stockRows = stockTask.Result;

        var response = stockRows
            .Select(stock =>
            {
                products.TryGetValue(stock.IdProducto, out var product);

                return new KardexResponse(
                    stock.IdProducto,
                    product?.NombreProducto ?? $"Producto {stock.IdProducto}",
                    stock.StockActual,
                    product?.Costo ?? 0m,
                    product?.PrecioVenta ?? 0m
                );
            })
            .OrderBy(x => x.NombreProducto)
            .ToList();

        return response;
    }

    public Task<IReadOnlyList<KardexMovementResponse>> GetProductMovementsAsync(int productId, CancellationToken cancellationToken = default)
    {
        return _movementGateway.GetHistoryByProductIdAsync(productId, cancellationToken);
    }
}
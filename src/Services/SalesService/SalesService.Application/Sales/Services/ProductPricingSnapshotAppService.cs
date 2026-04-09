using SalesService.Application.Sales.DTOs;
using SalesService.Application.Sales.Interfaces;
using SalesService.Domain.Entities;

namespace SalesService.Application.Sales.Services;

public sealed class ProductPricingSnapshotAppService : IProductPricingSnapshotAppService
{
    private readonly IProductPricingSnapshotRepository _repository;

    public ProductPricingSnapshotAppService(IProductPricingSnapshotRepository repository)
    {
        _repository = repository;
    }

    public async Task UpsertAsync(ProductPricingSnapshotUpsertRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(request.IdProducto, cancellationToken);

        if (existing is null)
        {
            var snapshot = new ProductPricingSnapshot(
                request.IdProducto,
                request.NombreProducto.Trim(),
                request.PrecioVenta
            );

            await _repository.AddAsync(snapshot, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return;
        }

        existing.Update(
            request.NombreProducto.Trim(),
            request.PrecioVenta
        );

        await _repository.SaveChangesAsync(cancellationToken);
    }
}
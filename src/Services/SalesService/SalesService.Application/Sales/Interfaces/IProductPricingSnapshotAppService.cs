using SalesService.Application.Sales.DTOs;

namespace SalesService.Application.Sales.Interfaces;

public interface IProductPricingSnapshotAppService
{
    Task UpsertAsync(ProductPricingSnapshotUpsertRequest request, CancellationToken cancellationToken = default);
}
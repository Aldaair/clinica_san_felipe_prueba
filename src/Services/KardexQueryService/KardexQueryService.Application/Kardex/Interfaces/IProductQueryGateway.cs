using KardexQueryService.Application.Kardex.DTOs;

namespace KardexQueryService.Application.Kardex.Interfaces;

public interface IProductQueryGateway
{
    Task<IReadOnlyList<ProductDto>> GetProductsAsync(CancellationToken cancellationToken = default);
}
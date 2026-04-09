using SalesService.Application.Sales.DTOs;

namespace SalesService.Application.Sales.Interfaces;

public interface ISaleAppService
{
    Task<SaleResponse> CreateAsync(CreateSaleRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SaleResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SaleResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SaleResponse?> CancelAsync(int id, CancellationToken cancellationToken = default);
}
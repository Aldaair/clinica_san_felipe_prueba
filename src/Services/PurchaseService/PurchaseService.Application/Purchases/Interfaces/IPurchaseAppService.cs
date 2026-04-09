using PurchaseService.Application.Purchases.DTOs;

namespace PurchaseService.Application.Purchases.Interfaces;

public interface IPurchaseAppService
{
    Task<PurchaseResponse> CreateAsync(CreatePurchaseRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PurchaseResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PurchaseResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PurchaseResponse?> CancelAsync(int id, CancellationToken cancellationToken = default);
}
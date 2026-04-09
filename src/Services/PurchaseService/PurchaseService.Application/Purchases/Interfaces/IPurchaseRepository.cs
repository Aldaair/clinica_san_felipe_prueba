using PurchaseService.Domain.Entities;

namespace PurchaseService.Application.Purchases.Interfaces;

public interface IPurchaseRepository
{
    Task AddAsync(PurchaseCab purchase, CancellationToken cancellationToken = default);
    Task<PurchaseCab?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PurchaseCab>> GetAllAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
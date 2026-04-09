using Microsoft.EntityFrameworkCore;
using PurchaseService.Application.Purchases.Interfaces;
using PurchaseService.Domain.Entities;
using PurchaseService.Infrastructure.Persistence;

namespace PurchaseService.Infrastructure.Purchases;

public sealed class PurchaseRepository : IPurchaseRepository
{
    private readonly PurchaseDbContext _dbContext;

    public PurchaseRepository(PurchaseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(PurchaseCab purchase, CancellationToken cancellationToken = default)
    {
        await _dbContext.PurchaseCabs.AddAsync(purchase, cancellationToken);
    }

    public async Task<PurchaseCab?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PurchaseCabs
            .Include(x => x.Details)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<PurchaseCab>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.PurchaseCabs
            .AsNoTracking()
            .Include(x => x.Details)
            .OrderByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
using Microsoft.EntityFrameworkCore;
using SalesService.Application.Sales.Interfaces;
using SalesService.Domain.Entities;
using SalesService.Infrastructure.Persistence;

namespace SalesService.Infrastructure.Sales;

public sealed class SaleRepository : ISaleRepository
{
    private readonly SalesDbContext _dbContext;

    public SaleRepository(SalesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(SaleCab sale, CancellationToken cancellationToken = default)
    {
        await _dbContext.SaleCabs.AddAsync(sale, cancellationToken);
    }

    public async Task<SaleCab?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaleCabs
            .Include(x => x.Details)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<SaleCab>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaleCabs
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
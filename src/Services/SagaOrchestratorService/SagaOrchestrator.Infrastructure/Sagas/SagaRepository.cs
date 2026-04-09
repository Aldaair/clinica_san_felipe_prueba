using Microsoft.EntityFrameworkCore;
using SagaOrchestrator.Application.Sagas.Interfaces;
using SagaOrchestrator.Domain.Entities;
using SagaOrchestrator.Infrastructure.Persistence;

namespace SagaOrchestrator.Infrastructure.Sagas;

public sealed class SagaRepository : ISagaRepository
{
    private readonly SagaDbContext _dbContext;

    public SagaRepository(SagaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(SagaInstance saga, CancellationToken cancellationToken = default)
    {
        await _dbContext.SagaInstances.AddAsync(saga, cancellationToken);
    }

    public async Task<SagaInstance?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.SagaInstances
            .AsNoTracking()
            .Include(x => x.Steps)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
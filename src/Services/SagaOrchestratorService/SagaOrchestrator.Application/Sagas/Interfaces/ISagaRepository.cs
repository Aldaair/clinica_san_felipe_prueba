using SagaOrchestrator.Domain.Entities;

namespace SagaOrchestrator.Application.Sagas.Interfaces;

public interface ISagaRepository
{
    Task AddAsync(SagaInstance saga, CancellationToken cancellationToken = default);
    Task<SagaInstance?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
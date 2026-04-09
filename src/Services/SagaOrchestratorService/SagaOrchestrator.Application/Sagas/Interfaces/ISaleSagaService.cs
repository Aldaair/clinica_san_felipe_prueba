using SagaOrchestrator.Application.Sagas.DTOs;

namespace SagaOrchestrator.Application.Sagas.Interfaces;

public interface ISaleSagaService
{
    Task<SaleSagaResponse> CreateAsync(CreateSaleSagaRequest request, CancellationToken cancellationToken = default);
    Task<SaleSagaResponse?> GetByIdAsync(int sagaId, CancellationToken cancellationToken = default);
}
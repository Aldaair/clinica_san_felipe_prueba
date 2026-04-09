using SagaOrchestrator.Application.Sagas.DTOs;

namespace SagaOrchestrator.Application.Sagas.Interfaces;

public interface ISaleServiceClient
{
    Task<SaleCreateResponse> CreateSaleAsync(CreateSaleSagaRequest request, CancellationToken cancellationToken = default);
    Task CancelSaleAsync(int saleId, CancellationToken cancellationToken = default);
}
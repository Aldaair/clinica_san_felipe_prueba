using System.Text.Json;
using SagaOrchestrator.Application.Sagas.DTOs;
using SagaOrchestrator.Application.Sagas.Interfaces;
using SagaOrchestrator.Domain.Entities;
using SagaOrchestrator.Domain.Enums;

namespace SagaOrchestrator.Application.Sagas.Services;

public sealed class PurchaseSagaService : IPurchaseSagaService
{
    private readonly ISagaRepository _sagaRepository;
    private readonly IPurchaseServiceClient _purchaseServiceClient;
    private readonly IProductServiceClient _productServiceClient;
    private readonly IMovementServiceClient _movementServiceClient;

    public PurchaseSagaService(
        ISagaRepository sagaRepository,
        IPurchaseServiceClient purchaseServiceClient,
        IProductServiceClient productServiceClient,
        IMovementServiceClient movementServiceClient)
    {
        _sagaRepository = sagaRepository;
        _purchaseServiceClient = purchaseServiceClient;
        _productServiceClient = productServiceClient;
        _movementServiceClient = movementServiceClient;
    }

    public async Task<PurchaseSagaResponse> CreateAsync(CreatePurchaseSagaRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Items is null || request.Items.Count == 0)
            throw new InvalidOperationException("La saga de compra requiere al menos un item.");

        var saga = new SagaInstance(SagaType.Purchase);

        await _sagaRepository.AddAsync(saga, cancellationToken);
        await _sagaRepository.SaveChangesAsync(cancellationToken);

        int? purchaseId = null;
        bool purchaseCreated = false;
        bool productUpdated = false;
        bool movementRegistered = false;

        try
        {
            saga.MarkInProgress("Purchase.Create");
            await _sagaRepository.SaveChangesAsync(cancellationToken);

            var purchaseResponse = await _purchaseServiceClient.CreatePurchaseAsync(request, cancellationToken);
            purchaseId = purchaseResponse.IdCompraCab;
            purchaseCreated = true;

            saga.AddStep(
                stepName: "Purchase.Create",
                status: SagaStepStatus.Completed,
                requestPayload: Serialize(request),
                responsePayload: Serialize(purchaseResponse),
                errorMessage: null);

            await _sagaRepository.SaveChangesAsync(cancellationToken);

            saga.MarkInProgress("Product.PurchaseBatchUpdate");
            await _sagaRepository.SaveChangesAsync(cancellationToken);

            var productUpdateRequest = new ProductPurchaseBatchUpdateRequest
            {
                PurchaseId = purchaseResponse.IdCompraCab,
                Items = request.Items.Select(x => new ProductPurchaseBatchUpdateItemRequest
                {
                    IdProducto = x.IdProducto,
                    NuevoCosto = x.Precio
                }).ToList()
            };

            await _productServiceClient.ApplyPurchaseBatchUpdateAsync(productUpdateRequest, cancellationToken);
            productUpdated = true;

            saga.AddStep(
                stepName: "Product.PurchaseBatchUpdate",
                status: SagaStepStatus.Completed,
                requestPayload: Serialize(productUpdateRequest),
                responsePayload: "{\"message\":\"Product batch update completed\"}",
                errorMessage: null);

            await _sagaRepository.SaveChangesAsync(cancellationToken);

            saga.MarkInProgress("Movement.Entry");
            await _sagaRepository.SaveChangesAsync(cancellationToken);

            var movementRequest = new MovementEntryRequest
            {
                IdDocumentoOrigen = purchaseResponse.IdCompraCab,
                IdTipoMovimiento = 1,
                Items = request.Items.Select(x => new MovementEntryItemRequest
                {
                    IdProducto = x.IdProducto,
                    Cantidad = x.Cantidad
                }).ToList()
            };

            await _movementServiceClient.RegisterEntryAsync(movementRequest, cancellationToken);
            movementRegistered = true;

            saga.AddStep(
                stepName: "Movement.Entry",
                status: SagaStepStatus.Completed,
                requestPayload: Serialize(movementRequest),
                responsePayload: "{\"message\":\"Movement entry completed\"}",
                errorMessage: null);

            saga.MarkCompleted("Completed");
            await _sagaRepository.SaveChangesAsync(cancellationToken);

            return Map(saga);
        }
        catch (Exception ex)
        {
            saga.MarkFailed(saga.CurrentStep, ex.Message);

            saga.AddStep(
                stepName: $"ERROR::{saga.CurrentStep}",
                status: SagaStepStatus.Failed,
                requestPayload: null,
                responsePayload: null,
                errorMessage: ex.Message);

            await _sagaRepository.SaveChangesAsync(cancellationToken);

            if (movementRegistered && purchaseId.HasValue)
            {
                try
                {
                    var reverseMovementRequest = new MovementReverseEntryRequest
                    {
                        IdDocumentoOrigen = purchaseId.Value,
                        IdTipoMovimiento = 1
                    };

                    await _movementServiceClient.ReverseEntryAsync(reverseMovementRequest, cancellationToken);

                    saga.AddStep(
                        stepName: "Movement.Entry.Reverse",
                        status: SagaStepStatus.Compensated,
                        requestPayload: Serialize(reverseMovementRequest),
                        responsePayload: "{\"message\":\"Movement entry reversed\"}",
                        errorMessage: null,
                        isCompensation: true);

                    await _sagaRepository.SaveChangesAsync(cancellationToken);
                }
                catch (Exception compensationEx)
                {
                    saga.AddStep(
                        stepName: "Movement.Entry.Reverse",
                        status: SagaStepStatus.Failed,
                        requestPayload: null,
                        responsePayload: null,
                        errorMessage: compensationEx.Message,
                        isCompensation: true);

                    await _sagaRepository.SaveChangesAsync(cancellationToken);
                }
            }

            if (productUpdated && purchaseId.HasValue)
            {
                try
                {
                    var rollbackRequest = new ProductPurchaseBatchRollbackRequest
                    {
                        PurchaseId = purchaseId.Value
                    };

                    await _productServiceClient.RollbackPurchaseBatchUpdateAsync(rollbackRequest, cancellationToken);

                    saga.AddStep(
                        stepName: "Product.PurchaseBatchUpdate.Rollback",
                        status: SagaStepStatus.Compensated,
                        requestPayload: Serialize(rollbackRequest),
                        responsePayload: "{\"message\":\"Product batch update rollback completed\"}",
                        errorMessage: null,
                        isCompensation: true);

                    await _sagaRepository.SaveChangesAsync(cancellationToken);
                }
                catch (Exception compensationEx)
                {
                    saga.AddStep(
                        stepName: "Product.PurchaseBatchUpdate.Rollback",
                        status: SagaStepStatus.Failed,
                        requestPayload: null,
                        responsePayload: null,
                        errorMessage: compensationEx.Message,
                        isCompensation: true);

                    await _sagaRepository.SaveChangesAsync(cancellationToken);
                }
            }

            if (purchaseCreated && purchaseId.HasValue)
            {
                try
                {
                    await _purchaseServiceClient.CancelPurchaseAsync(purchaseId.Value, cancellationToken);

                    saga.AddStep(
                        stepName: "Purchase.Cancel",
                        status: SagaStepStatus.Compensated,
                        requestPayload: $"{{\"purchaseId\":{purchaseId.Value}}}",
                        responsePayload: "{\"message\":\"Purchase cancelled\"}",
                        errorMessage: null,
                        isCompensation: true);

                    saga.MarkCompensated("Compensated");
                    await _sagaRepository.SaveChangesAsync(cancellationToken);
                }
                catch (Exception compensationEx)
                {
                    saga.AddStep(
                        stepName: "Purchase.Cancel",
                        status: SagaStepStatus.Failed,
                        requestPayload: null,
                        responsePayload: null,
                        errorMessage: compensationEx.Message,
                        isCompensation: true);

                    await _sagaRepository.SaveChangesAsync(cancellationToken);
                }
            }

            return Map(saga);
        }
    }

    public async Task<PurchaseSagaResponse?> GetByIdAsync(int sagaId, CancellationToken cancellationToken = default)
    {
        var saga = await _sagaRepository.GetByIdAsync(sagaId, cancellationToken);
        return saga is null ? null : Map(saga);
    }

    private static PurchaseSagaResponse Map(SagaInstance saga)
    {
        return new PurchaseSagaResponse(
            saga.Id,
            saga.CorrelationId,
            saga.SagaType.ToString(),
            saga.Status.ToString(),
            saga.CurrentStep,
            saga.ErrorMessage,
            saga.CreatedAtUtc,
            saga.UpdatedAtUtc
        );
    }

    private static string Serialize<T>(T value) => JsonSerializer.Serialize(value);
}
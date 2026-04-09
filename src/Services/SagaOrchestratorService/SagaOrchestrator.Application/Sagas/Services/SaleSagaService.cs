using System.Text.Json;
using SagaOrchestrator.Application.Sagas.DTOs;
using SagaOrchestrator.Application.Sagas.Interfaces;
using SagaOrchestrator.Domain.Entities;
using SagaOrchestrator.Domain.Enums;

namespace SagaOrchestrator.Application.Sagas.Services;

public sealed class SaleSagaService : ISaleSagaService
{
    private readonly ISagaRepository _sagaRepository;
    private readonly ISaleServiceClient _saleServiceClient;
    private readonly IMovementServiceClient _movementServiceClient;

    public SaleSagaService(
        ISagaRepository sagaRepository,
        ISaleServiceClient saleServiceClient,
        IMovementServiceClient movementServiceClient)
    {
        _sagaRepository = sagaRepository;
        _saleServiceClient = saleServiceClient;
        _movementServiceClient = movementServiceClient;
    }

    public async Task<SaleSagaResponse> CreateAsync(CreateSaleSagaRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Items is null || request.Items.Count == 0)
            throw new InvalidOperationException("La saga de venta requiere al menos un item.");

        var saga = new SagaInstance(SagaType.Sale);

        await _sagaRepository.AddAsync(saga, cancellationToken);
        await _sagaRepository.SaveChangesAsync(cancellationToken);

        int? saleId = null;
        bool saleCreated = false;
        bool exitRegistered = false;

        try
        {
            saga.MarkInProgress("Movement.StockValidation");
            await _sagaRepository.SaveChangesAsync(cancellationToken);

            foreach (var item in request.Items)
            {
                var stock = await _movementServiceClient.GetStockByProductIdAsync(item.IdProducto, cancellationToken);

                if (item.Cantidad > stock.StockActual)
                    throw new InvalidOperationException(
                        $"Stock insuficiente para el producto {item.IdProducto}. Stock actual: {stock.StockActual}");
            }

            saga.AddStep(
                stepName: "Movement.StockValidation",
                status: SagaStepStatus.Completed,
                requestPayload: Serialize(request),
                responsePayload: "{\"message\":\"Stock validation completed\"}",
                errorMessage: null);

            await _sagaRepository.SaveChangesAsync(cancellationToken);

            saga.MarkInProgress("Sale.Create");
            await _sagaRepository.SaveChangesAsync(cancellationToken);

            var saleResponse = await _saleServiceClient.CreateSaleAsync(request, cancellationToken);
            saleId = saleResponse.IdVentaCab;
            saleCreated = true;

            saga.AddStep(
                stepName: "Sale.Create",
                status: SagaStepStatus.Completed,
                requestPayload: Serialize(request),
                responsePayload: Serialize(saleResponse),
                errorMessage: null);

            await _sagaRepository.SaveChangesAsync(cancellationToken);

            saga.MarkInProgress("Movement.Exit");
            await _sagaRepository.SaveChangesAsync(cancellationToken);

            var movementRequest = new MovementExitRequest
            {
                IdDocumentoOrigen = saleResponse.IdVentaCab,
                IdTipoMovimiento = 2,
                Items = request.Items.Select(x => new MovementExitItemRequest
                {
                    IdProducto = x.IdProducto,
                    Cantidad = x.Cantidad
                }).ToList()
            };

            await _movementServiceClient.RegisterExitAsync(movementRequest, cancellationToken);
            exitRegistered = true;

            saga.AddStep(
                stepName: "Movement.Exit",
                status: SagaStepStatus.Completed,
                requestPayload: Serialize(movementRequest),
                responsePayload: "{\"message\":\"Movement exit completed\"}",
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

            if (exitRegistered && saleId.HasValue)
            {
                try
                {
                    var reverseExitRequest = new MovementReverseExitRequest
                    {
                        IdDocumentoOrigen = saleId.Value,
                        IdTipoMovimiento = 2
                    };

                    await _movementServiceClient.ReverseExitAsync(reverseExitRequest, cancellationToken);

                    saga.AddStep(
                        stepName: "Movement.Exit.Reverse",
                        status: SagaStepStatus.Compensated,
                        requestPayload: Serialize(reverseExitRequest),
                        responsePayload: "{\"message\":\"Movement exit reversed\"}",
                        errorMessage: null,
                        isCompensation: true);

                    await _sagaRepository.SaveChangesAsync(cancellationToken);
                }
                catch (Exception compensationEx)
                {
                    saga.AddStep(
                        stepName: "Movement.Exit.Reverse",
                        status: SagaStepStatus.Failed,
                        requestPayload: null,
                        responsePayload: null,
                        errorMessage: compensationEx.Message,
                        isCompensation: true);

                    await _sagaRepository.SaveChangesAsync(cancellationToken);
                }
            }

            if (saleCreated && saleId.HasValue)
            {
                try
                {
                    await _saleServiceClient.CancelSaleAsync(saleId.Value, cancellationToken);

                    saga.AddStep(
                        stepName: "Sale.Cancel",
                        status: SagaStepStatus.Compensated,
                        requestPayload: $"{{\"saleId\":{saleId.Value}}}",
                        responsePayload: "{\"message\":\"Sale cancelled\"}",
                        errorMessage: null,
                        isCompensation: true);

                    saga.MarkCompensated("Compensated");
                    await _sagaRepository.SaveChangesAsync(cancellationToken);
                }
                catch (Exception compensationEx)
                {
                    saga.AddStep(
                        stepName: "Sale.Cancel",
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

    public async Task<SaleSagaResponse?> GetByIdAsync(int sagaId, CancellationToken cancellationToken = default)
    {
        var saga = await _sagaRepository.GetByIdAsync(sagaId, cancellationToken);
        return saga is null ? null : Map(saga);
    }

    private static SaleSagaResponse Map(SagaInstance saga)
    {
        return new SaleSagaResponse(
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
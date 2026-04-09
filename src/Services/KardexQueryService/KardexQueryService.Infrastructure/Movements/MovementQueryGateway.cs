using System.Net.Http.Json;
using KardexQueryService.Application.Kardex.DTOs;
using KardexQueryService.Application.Kardex.Interfaces;

namespace KardexQueryService.Infrastructure.Movements;

public sealed class MovementQueryGateway : IMovementQueryGateway
{
    private readonly HttpClient _httpClient;

    public MovementQueryGateway(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<StockDto>> GetCurrentStockAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<List<StockApiResponse>>("/api/movements/stock", cancellationToken)
                       ?? new List<StockApiResponse>();

        return response
            .Select(x => new StockDto(x.IdProducto, x.StockActual))
            .ToList();
    }

    public async Task<IReadOnlyList<KardexMovementResponse>> GetHistoryByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<List<MovementHistoryApiResponse>>($"/api/movements/history/{productId}", cancellationToken)
                       ?? new List<MovementHistoryApiResponse>();

        return response
            .Select(x => new KardexMovementResponse(
                x.FechaRegistro,
                x.TipoMovimiento,
                x.Cantidad
            ))
            .ToList();
    }

    private sealed class StockApiResponse
    {
        public int IdProducto { get; set; }
        public decimal StockActual { get; set; }
    }

    private sealed class MovementHistoryApiResponse
    {
        public DateTime FechaRegistro { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
    }
}
using System.Net.Http.Json;
using KardexQueryService.Application.Kardex.DTOs;
using KardexQueryService.Application.Kardex.Interfaces;

namespace KardexQueryService.Infrastructure.Products;

public sealed class ProductQueryGateway : IProductQueryGateway
{
    private readonly HttpClient _httpClient;

    public ProductQueryGateway(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<ProductDto>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<List<ProductApiResponse>>("/api/products", cancellationToken)
                       ?? new List<ProductApiResponse>();

        return response
            .Select(x => new ProductDto(
                x.Id,
                x.NombreProducto,
                x.Costo,
                x.PrecioVenta
            ))
            .ToList();
    }

    private sealed class ProductApiResponse
    {
        public int Id { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public string NroLote { get; set; } = string.Empty;
        public DateTime FecRegistro { get; set; }
        public decimal Costo { get; set; }
        public decimal PrecioVenta { get; set; }
    }
}
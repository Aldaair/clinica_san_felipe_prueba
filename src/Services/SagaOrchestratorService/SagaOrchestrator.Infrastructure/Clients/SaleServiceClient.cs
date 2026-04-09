using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using SagaOrchestrator.Application.Sagas.DTOs;
using SagaOrchestrator.Application.Sagas.Interfaces;

namespace SagaOrchestrator.Infrastructure.Clients;

public sealed class SaleServiceClient : ISaleServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SaleServiceClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<SaleCreateResponse> CreateSaleAsync(CreateSaleSagaRequest request, CancellationToken cancellationToken = default)
    {
        AddBearerToken();

        var response = await _httpClient.PostAsJsonAsync("/api/sales", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<SaleCreateResponse>(cancellationToken: cancellationToken)
                      ?? throw new InvalidOperationException("Respuesta inválida de SalesService.");

        return payload;
    }

    public async Task CancelSaleAsync(int saleId, CancellationToken cancellationToken = default)
    {
        AddBearerToken();

        var response = await _httpClient.PostAsync($"/api/sales/{saleId}/cancel", null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private void AddBearerToken()
    {
        var token = BearerTokenHelper.GetBearerToken(_httpContextAccessor);

        _httpClient.DefaultRequestHeaders.Authorization = null;

        if (!string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
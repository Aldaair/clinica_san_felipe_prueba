using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using SagaOrchestrator.Application.Sagas.DTOs;
using SagaOrchestrator.Application.Sagas.Interfaces;

namespace SagaOrchestrator.Infrastructure.Clients;

public sealed class PurchaseServiceClient : IPurchaseServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PurchaseServiceClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PurchaseCreateResponse> CreatePurchaseAsync(CreatePurchaseSagaRequest request, CancellationToken cancellationToken = default)
    {
        AddBearerToken();

        var response = await _httpClient.PostAsJsonAsync("/api/purchases", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<PurchaseCreateResponse>(cancellationToken: cancellationToken)
                      ?? throw new InvalidOperationException("Respuesta inválida de PurchaseService.");

        return payload;
    }

    public async Task CancelPurchaseAsync(int purchaseId, CancellationToken cancellationToken = default)
    {
        AddBearerToken();

        var response = await _httpClient.PostAsync($"/api/purchases/{purchaseId}/cancel", null, cancellationToken);
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
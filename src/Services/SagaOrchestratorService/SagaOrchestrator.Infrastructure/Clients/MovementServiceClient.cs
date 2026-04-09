using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using SagaOrchestrator.Application.Sagas.DTOs;
using SagaOrchestrator.Application.Sagas.Interfaces;

namespace SagaOrchestrator.Infrastructure.Clients;

public sealed class MovementServiceClient : IMovementServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MovementServiceClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task RegisterEntryAsync(MovementEntryRequest request, CancellationToken cancellationToken = default)
    {
        AddBearerToken();

        var response = await _httpClient.PostAsJsonAsync("/api/movements/entry", request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task ReverseEntryAsync(MovementReverseEntryRequest request, CancellationToken cancellationToken = default)
    {
        AddBearerToken();

        var response = await _httpClient.PostAsJsonAsync("/api/movements/entry/reverse", request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task RegisterExitAsync(MovementExitRequest request, CancellationToken cancellationToken = default)
    {
        AddBearerToken();

        var response = await _httpClient.PostAsJsonAsync("/api/movements/exit", request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task ReverseExitAsync(MovementReverseExitRequest request, CancellationToken cancellationToken = default)
    {
        AddBearerToken();

        var response = await _httpClient.PostAsJsonAsync("/api/movements/exit/reverse", request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<ProductStockResponse> GetStockByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        AddBearerToken();

        var response = await _httpClient.GetAsync($"/api/movements/stock/{productId}", cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<ProductStockResponse>(cancellationToken: cancellationToken)
                      ?? throw new InvalidOperationException("Respuesta inválida de MovementService.");

        return payload;
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
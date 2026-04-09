using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using SagaOrchestrator.Application.Sagas.DTOs;
using SagaOrchestrator.Application.Sagas.Interfaces;

namespace SagaOrchestrator.Infrastructure.Clients;

public sealed class ProductServiceClient : IProductServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProductServiceClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task ApplyPurchaseBatchUpdateAsync(ProductPurchaseBatchUpdateRequest request, CancellationToken cancellationToken = default)
    {
        AddBearerToken();

        var response = await _httpClient.PostAsJsonAsync("/api/products/purchase-batch-update", request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task RollbackPurchaseBatchUpdateAsync(ProductPurchaseBatchRollbackRequest request, CancellationToken cancellationToken = default)
    {
        AddBearerToken();

        var response = await _httpClient.PostAsJsonAsync("/api/products/purchase-batch-update/rollback", request, cancellationToken);
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
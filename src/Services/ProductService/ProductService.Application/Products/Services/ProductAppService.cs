using ProductService.Application.Products.DTOs;
using ProductService.Application.Products.Interfaces;
using ProductService.Domain.Entities;
using SanFelipe.IntegrationEvents;
using System.Text.Json;


namespace ProductService.Application.Products.Services;

public sealed class ProductAppService : IProductAppService
{
    private readonly IProductRepository _productRepository;

    public ProductAppService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {


        var nombreProducto = request.NombreProducto.Trim();
        var nroLote = request.NroLote.Trim();

        var exists = await _productRepository.ExistsByNameAndLotAsync(
            nombreProducto,
            nroLote,
            cancellationToken);

        if (exists)
            throw new InvalidOperationException("Ya existe un producto con el mismo nombre y lote.");

        var product = new Product(
            nombreProducto,
            nroLote,
            DateTime.UtcNow,
            request.Costo,
            request.PrecioVenta);

        await _productRepository.ExecuteInTransactionAsync(async ct =>
        {
            // 1. Agregar producto
            await _productRepository.AddAsync(product, ct);

            // 2. Guardar para obtener el Id generado por la BD
            await _productRepository.SaveChangesAsync(ct);

            // 3. Crear evento de integración
            var integrationEvent = new ProductPricingChangedIntegrationEvent(
                EventId: Guid.NewGuid(),
                OccurredAtUtc: DateTime.UtcNow,
                IdProducto: product.Id,
                NombreProducto: product.NombreProducto,
                PrecioVenta: product.PrecioVenta
            );

            // 4. Guardar evento en outbox
            var outboxMessage = new OutboxMessage
            {
                Id = integrationEvent.EventId,
                EventType = nameof(ProductPricingChangedIntegrationEvent),
                Payload = JsonSerializer.Serialize(integrationEvent),
                OccurredAtUtc = integrationEvent.OccurredAtUtc
            };

            await _productRepository.AddOutboxMessageAsync(outboxMessage, ct);

            // 5. Guardar outbox
            await _productRepository.SaveChangesAsync(ct);
        }, cancellationToken);

        return Map(product);
    }

    public async Task<ProductResponse?> UpdateAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);

        if (product is null)
            return null;

        product.Update(
            request.NombreProducto.Trim(),
            request.NroLote.Trim(),
            request.Costo,
            request.PrecioVenta);

        await _productRepository.SaveChangesAsync(cancellationToken);

        return Map(product);
    }

    public async Task<IReadOnlyList<ProductResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        return products.Select(Map).ToList();
    }

    public async Task<ProductResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        return product is null ? null : Map(product);
    }

    private static ProductResponse Map(Product product)
    {
        return new ProductResponse(
            product.Id,
            product.NombreProducto,
            product.NroLote,
            product.FecRegistro,
            product.Costo,
            product.PrecioVenta);
    }
}
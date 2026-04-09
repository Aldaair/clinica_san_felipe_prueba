using SalesService.Application.Sales.DTOs;
using SalesService.Application.Sales.Interfaces;
using SalesService.Domain.Entities;
using SalesService.Domain.Enums;

namespace SalesService.Application.Sales.Services;

public sealed class SaleAppService : ISaleAppService
{
    private const decimal IgvRate = 0.18m;

    private readonly ISaleRepository _saleRepository;
    private readonly IProductPricingSnapshotRepository _productPricingSnapshotRepository;

    public SaleAppService(ISaleRepository saleRepository, IProductPricingSnapshotRepository productPricingSnapshotRepository)
    {
        _saleRepository = saleRepository;
        _productPricingSnapshotRepository = productPricingSnapshotRepository;
    }

    public async Task<SaleResponse> CreateAsync(CreateSaleRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Items is null || request.Items.Count == 0)
            throw new InvalidOperationException("La venta debe tener al menos un producto.");

        var productIds = request.Items
            .Select(x => x.IdProducto)
            .Distinct()
            .ToList();

        var snapshots = await _productPricingSnapshotRepository.GetByIdsAsync(productIds, cancellationToken);
        var snapshotById = snapshots.ToDictionary(x => x.IdProducto);

        var missingIds = productIds
            .Where(id => !snapshotById.ContainsKey(id))
            .ToList();

        if (missingIds.Count > 0)
            throw new InvalidOperationException(
                $"No existen snapshots de precio para los productos: {string.Join(", ", missingIds)}");

        decimal subTotal = 0m;
        decimal igv = 0m;
        decimal total = 0m;

        var detailDrafts = new List<(int IdProducto, decimal Cantidad, decimal Precio, decimal SubTotal, decimal Igv, decimal Total)>();

        foreach (var item in request.Items)
        {
            if (item.Cantidad <= 0)
                throw new InvalidOperationException("La cantidad debe ser mayor que cero.");

            var snapshot = snapshotById[item.IdProducto];
            var precioVenta = snapshot.PrecioVenta;

            if (precioVenta <= 0)
                throw new InvalidOperationException($"El producto {item.IdProducto} no tiene precio de venta válido.");

            var lineSubTotal = Round(item.Cantidad * precioVenta);
            var lineIgv = Round(lineSubTotal * IgvRate);
            var lineTotal = Round(lineSubTotal + lineIgv);

            subTotal += lineSubTotal;
            igv += lineIgv;
            total += lineTotal;

            detailDrafts.Add((
                item.IdProducto,
                item.Cantidad,
                precioVenta,
                lineSubTotal,
                lineIgv,
                lineTotal
            ));
        }

        var sale = new SaleCab(
            DateTime.UtcNow,
            Round(subTotal),
            Round(igv),
            Round(total)
        );

        foreach (var draft in detailDrafts)
        {
            sale.AddDetail(
                draft.IdProducto,
                draft.Cantidad,
                draft.Precio,
                draft.SubTotal,
                draft.Igv,
                draft.Total
            );
        }

        await _saleRepository.AddAsync(sale, cancellationToken);
        await _saleRepository.SaveChangesAsync(cancellationToken);

        return Map(sale, "CREATED");
    }
    public async Task<IReadOnlyList<SaleResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sales = await _saleRepository.GetAllAsync(cancellationToken);
        return sales.Select(sale => Map(sale, "ACTIVE")).ToList();
    }

    public async Task<SaleResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sale = await _saleRepository.GetByIdAsync(id, cancellationToken);
        return sale is null ? null : Map(sale, "ACTIVE");
    }

    public async Task<SaleResponse?> CancelAsync(int id, CancellationToken cancellationToken = default)
    {
        var sale = await _saleRepository.GetByIdAsync(id, cancellationToken);

        if (sale is null)
            return null;

        if (sale.Status == SaleStatus.Cancelled)
            return Map(sale, "CANCELLED");

        sale.Cancel();

        await _saleRepository.SaveChangesAsync(cancellationToken);

        return Map(sale, "CANCELLED");
    }

    private static SaleResponse Map(SaleCab sale, string status = "CREATED")
    {
        return new SaleResponse(
            sale.Id,
            sale.FecRegistro,
            sale.SubTotal,
            sale.Igv,
            sale.Total,
            status,
            sale.Details.Select(d => new SaleDetailResponse(
                d.IdProducto,
                d.Cantidad,
                d.Precio,
                d.SubTotal,
                d.Igv,
                d.Total
            )).ToList()
        );
    }

    private static decimal Round(decimal value)
        => decimal.Round(value, 2, MidpointRounding.AwayFromZero);
}
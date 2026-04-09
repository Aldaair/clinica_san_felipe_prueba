using PurchaseService.Application.Purchases.DTOs;
using PurchaseService.Application.Purchases.Interfaces;
using PurchaseService.Domain.Entities;
using PurchaseService.Domain.Enums;

namespace PurchaseService.Application.Purchases.Services;

public sealed class PurchaseAppService : IPurchaseAppService
{
    private const decimal IgvRate = 0.18m;

    private readonly IPurchaseRepository _purchaseRepository;

    public PurchaseAppService(IPurchaseRepository purchaseRepository)
    {
        _purchaseRepository = purchaseRepository;
    }

    public async Task<PurchaseResponse> CreateAsync(CreatePurchaseRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Items is null || request.Items.Count == 0)
            throw new InvalidOperationException("La compra debe tener al menos un producto.");

        decimal subTotal = 0m;
        decimal igv = 0m;
        decimal total = 0m;

        var purchase = new PurchaseCab(
            DateTime.UtcNow,
            0m,
            0m,
            0m
        );

        foreach (var item in request.Items)
        {
            if (item.Cantidad <= 0)
                throw new InvalidOperationException("La cantidad debe ser mayor que cero.");

            if (item.Precio <= 0)
                throw new InvalidOperationException("El precio debe ser mayor que cero.");

            var lineSubTotal = Round(item.Cantidad * item.Precio);
            var lineIgv = Round(lineSubTotal * IgvRate);
            var lineTotal = Round(lineSubTotal + lineIgv);

            subTotal += lineSubTotal;
            igv += lineIgv;
            total += lineTotal;

            purchase.AddDetail(
                item.IdProducto,
                item.Cantidad,
                item.Precio,
                lineSubTotal,
                lineIgv,
                lineTotal
            );
        }

        purchase = new PurchaseCab(
            DateTime.UtcNow,
            Round(subTotal),
            Round(igv),
            Round(total)
        );

        foreach (var item in request.Items)
        {
            var lineSubTotal = Round(item.Cantidad * item.Precio);
            var lineIgv = Round(lineSubTotal * IgvRate);
            var lineTotal = Round(lineSubTotal + lineIgv);

            purchase.AddDetail(
                item.IdProducto,
                item.Cantidad,
                item.Precio,
                lineSubTotal,
                lineIgv,
                lineTotal
            );
        }

        await _purchaseRepository.AddAsync(purchase, cancellationToken);
        await _purchaseRepository.SaveChangesAsync(cancellationToken);

        return Map(purchase);
    }

    public async Task<IReadOnlyList<PurchaseResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var purchases = await _purchaseRepository.GetAllAsync(cancellationToken);
        return purchases.Select(Map).ToList();
    }

    public async Task<PurchaseResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var purchase = await _purchaseRepository.GetByIdAsync(id, cancellationToken);
        return purchase is null ? null : Map(purchase);
    }

    public async Task<PurchaseResponse?> CancelAsync(int id, CancellationToken cancellationToken = default)
    {
        var purchase = await _purchaseRepository.GetByIdAsync(id, cancellationToken);

        if (purchase is null)
            return null;

        if (purchase.Status == PurchaseStatus.Cancelled)
        
            return Map(purchase);

        purchase.Cancel();

        await _purchaseRepository.SaveChangesAsync(cancellationToken);

        return Map(purchase);
    }

    private static PurchaseResponse Map(PurchaseCab purchase)
    {
        return new PurchaseResponse(
            purchase.Id,
            purchase.FecRegistro,
            purchase.SubTotal,
            purchase.Igv,
            purchase.Total,
            purchase.Status.ToString(),
            purchase.Details.Select(d => new PurchaseDetailResponse(
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
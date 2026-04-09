using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Persistence;

public static class ProductDbSeeder
{
    public static async Task SeedAsync(ProductDbContext dbContext)
    {
        await dbContext.Database.EnsureCreatedAsync();

        if (await dbContext.Products.AnyAsync())
            return;

        var products = new List<Product>
        {
            new Product(
                nombreProducto: "Paracetamol 500mg",
                nroLote: "LOT-PARA-001",
                fecRegistro: DateTime.UtcNow,
                costo: 10.50m,
                precioVenta: 14.18m
            ),
            new Product(
                nombreProducto: "Ibuprofeno 400mg",
                nroLote: "LOT-IBU-001",
                fecRegistro: DateTime.UtcNow,
                costo: 12.00m,
                precioVenta: 16.20m
            ),
            new Product(
                nombreProducto: "Amoxicilina 500mg",
                nroLote: "LOT-AMOX-001",
                fecRegistro: DateTime.UtcNow,
                costo: 18.50m,
                precioVenta: 24.98m
            ),
            new Product(
                nombreProducto: "Omeprazol 20mg",
                nroLote: "LOT-OME-001",
                fecRegistro: DateTime.UtcNow,
                costo: 8.90m,
                precioVenta: 12.02m
            ),
            new Product(
                nombreProducto: "Loratadina 10mg",
                nroLote: "LOT-LORA-001",
                fecRegistro: DateTime.UtcNow,
                costo: 7.80m,
                precioVenta: 10.53m
            )
        };

        await dbContext.Products.AddRangeAsync(products);
        await dbContext.SaveChangesAsync();
    }
}
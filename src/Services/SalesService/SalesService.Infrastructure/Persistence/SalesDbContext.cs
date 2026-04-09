using Microsoft.EntityFrameworkCore;
using SalesService.Domain.Entities;

namespace SalesService.Infrastructure.Persistence;

public sealed class SalesDbContext : DbContext
{
    public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options)
    {
    }

    public DbSet<SaleCab> SaleCabs => Set<SaleCab>();
    public DbSet<SaleDet> SaleDets => Set<SaleDet>();
    public DbSet<ProductPricingSnapshot> ProductPricingSnapshots => Set<ProductPricingSnapshot>();
    public DbSet<ProcessedIntegrationEvent> ProcessedIntegrationEvents => Set<ProcessedIntegrationEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProcessedIntegrationEvent>(entity =>
        {
            entity.ToTable("ProcessedIntegrationEvents");
            entity.HasKey(x => x.EventId);

            entity.Property(x => x.EventId)
                .HasColumnName("EventId");

            entity.Property(x => x.EventType)
                .HasColumnName("EventType")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.ProcessedAtUtc)
                .HasColumnName("ProcessedAtUtc")
                .IsRequired();
        });

        modelBuilder.Entity<SaleCab>(entity =>
        {
            entity.ToTable("VentaCab");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("Id_VentaCab")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.FecRegistro)
                .HasColumnName("FecRegistro")
                .IsRequired();

            entity.Property(x => x.SubTotal)
                .HasColumnName("SubTotal")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(x => x.Igv)
                .HasColumnName("Igv")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(x => x.Total)
                .HasColumnName("Total")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(x => x.Status)
                .HasColumnName("Status")
                .HasConversion<int>()
                .IsRequired();

            entity.HasMany(x => x.Details)
                .WithOne()
                .HasForeignKey(x => x.IdVentaCab)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SaleDet>(entity =>
        {
            entity.ToTable("VentaDet");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("Id_VentaDet")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.IdVentaCab)
                .HasColumnName("Id_VentaCab")
                .IsRequired();

            entity.Property(x => x.IdProducto)
                .HasColumnName("Id_producto")
                .IsRequired();

            entity.Property(x => x.Cantidad)
                .HasColumnName("Cantidad")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(x => x.Precio)
                .HasColumnName("Precio")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(x => x.SubTotal)
                .HasColumnName("Sub_Total")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(x => x.Igv)
                .HasColumnName("Igv")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(x => x.Total)
                .HasColumnName("Total")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });

        modelBuilder.Entity<ProductPricingSnapshot>(entity =>
        {
            entity.ToTable("ProductPricingSnapshot");
            entity.HasKey(x => x.IdProducto);

            entity.Property(x => x.IdProducto)
                .HasColumnName("IdProducto");

            entity.Property(x => x.NombreProducto)
                .HasColumnName("NombreProducto")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.PrecioVenta)
                .HasColumnName("PrecioVenta")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(x => x.LastUpdatedUtc)
                .HasColumnName("LastUpdatedUtc")
                .IsRequired();
        });
    }
}
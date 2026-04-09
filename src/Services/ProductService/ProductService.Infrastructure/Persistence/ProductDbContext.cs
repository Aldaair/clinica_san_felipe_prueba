using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Persistence;

public sealed class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductPriceUpdateLog> ProductPriceUpdateLogs => Set<ProductPriceUpdateLog>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("OutboxMessages");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.EventType).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Payload).IsRequired();
            entity.Property(x => x.OccurredAtUtc).IsRequired();
            entity.Property(x => x.ErrorMessage).HasMaxLength(1000);
        });

        modelBuilder.Entity<ProductPriceUpdateLog>(entity =>
        {
            entity.ToTable("ProductPriceUpdateLog");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.PurchaseId)
                .HasColumnName("PurchaseId")
                .IsRequired();

            entity.Property(x => x.ProductId)
                .HasColumnName("ProductId")
                .IsRequired();

            entity.Property(x => x.OldCosto)
                .HasColumnName("OldCosto")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(x => x.OldPrecioVenta)
                .HasColumnName("OldPrecioVenta")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(x => x.NewCosto)
                .HasColumnName("NewCosto")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(x => x.NewPrecioVenta)
                .HasColumnName("NewPrecioVenta")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(x => x.RolledBack)
                .HasColumnName("RolledBack")
                .IsRequired();

            entity.Property(x => x.CreatedAtUtc)
                .HasColumnName("CreatedAtUtc")
                .IsRequired();

            entity.Property(x => x.RolledBackAtUtc)
                .HasColumnName("RolledBackAtUtc");

            entity.HasIndex(x => new { x.PurchaseId, x.ProductId }).IsUnique();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Productos");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("Id_producto")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.NombreProducto)
                .HasColumnName("Nombre_producto")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.NroLote)
                .HasColumnName("NroLote")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.FecRegistro)
                .HasColumnName("Fec_registro")
                .IsRequired();

            entity.Property(x => x.Costo)
                .HasColumnName("Costo")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(x => x.PrecioVenta)
                .HasColumnName("PrecioVenta")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });
    }
}
using Microsoft.EntityFrameworkCore;
using PurchaseService.Domain.Entities;
using PurchaseService.Domain.Enums;

namespace PurchaseService.Infrastructure.Persistence;

public sealed class PurchaseDbContext : DbContext
{
    public PurchaseDbContext(DbContextOptions<PurchaseDbContext> options) : base(options)
    {
    }

    public DbSet<PurchaseCab> PurchaseCabs => Set<PurchaseCab>();
    public DbSet<PurchaseDet> PurchaseDets => Set<PurchaseDet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PurchaseCab>(entity =>
        {
            entity.ToTable("CompraCab");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("Id_CompraCab")
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
                .HasForeignKey(x => x.IdCompraCab)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PurchaseDet>(entity =>
        {
            entity.ToTable("CompraDet");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("Id_CompraDet")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.IdCompraCab)
                .HasColumnName("Id_CompraCab")
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
    }
}
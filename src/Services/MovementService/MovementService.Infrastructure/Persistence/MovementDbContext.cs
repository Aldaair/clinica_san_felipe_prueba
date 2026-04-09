using Microsoft.EntityFrameworkCore;
using MovementService.Domain.Entities;
using MovementService.Domain.Enums;

namespace MovementService.Infrastructure.Persistence;

public sealed class MovementDbContext : DbContext
{
    public MovementDbContext(DbContextOptions<MovementDbContext> options) : base(options)
    {
    }

    public DbSet<MovimientoCab> MovimientoCabs => Set<MovimientoCab>();
    public DbSet<MovimientoDet> MovimientoDets => Set<MovimientoDet>();
    public DbSet<MovementCompensationLog> MovementCompensationLogs => Set<MovementCompensationLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MovimientoCab>(entity =>
        {
            entity.ToTable("MovimientoCab");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("Id_MovimientoCab")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.FecRegistro)
                .HasColumnName("Fec_registro")
                .IsRequired();

            entity.Property(x => x.IdTipoMovimiento)
                .HasColumnName("Id_TipoMovimiento")
                .HasConversion<int>()
                .IsRequired();

            entity.Property(x => x.IdDocumentoOrigen)
                .HasColumnName("Id_DocumentoOrigen")
                .IsRequired();

            entity.Property(x => x.IsCompensation)
                .HasColumnName("IsCompensation")
                .IsRequired();

            entity.HasMany(x => x.Details)
                .WithOne()
                .HasForeignKey(x => x.IdMovimientoCab)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MovimientoDet>(entity =>
        {
            entity.ToTable("MovimientoDet");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("Id_MovimientoDet")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.IdMovimientoCab)
                .HasColumnName("Id_movimientocab")
                .IsRequired();

            entity.Property(x => x.IdProducto)
                .HasColumnName("Id_Producto")
                .IsRequired();

            entity.Property(x => x.Cantidad)
                .HasColumnName("Cantidad")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });

        modelBuilder.Entity<MovementCompensationLog>(entity =>
        {
            entity.ToTable("MovementCompensationLog");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.IdDocumentoOrigen)
                .HasColumnName("IdDocumentoOrigen")
                .IsRequired();

            entity.Property(x => x.OriginalMovementType)
                .HasColumnName("OriginalMovementType")
                .HasConversion<int>()
                .IsRequired();

            entity.Property(x => x.CompensationMovementType)
                .HasColumnName("CompensationMovementType")
                .HasConversion<int>()
                .IsRequired();

            entity.Property(x => x.CreatedAtUtc)
                .HasColumnName("CreatedAtUtc")
                .IsRequired();

            entity.HasIndex(x => new { x.IdDocumentoOrigen, x.OriginalMovementType, x.CompensationMovementType })
                .IsUnique();
        });
    }
}
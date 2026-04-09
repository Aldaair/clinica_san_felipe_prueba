using Microsoft.EntityFrameworkCore;
using SagaOrchestrator.Domain.Entities;
using SagaOrchestrator.Domain.Enums;

namespace SagaOrchestrator.Infrastructure.Persistence;

public sealed class SagaDbContext : DbContext
{
    public SagaDbContext(DbContextOptions<SagaDbContext> options) : base(options)
    {
    }

    public DbSet<SagaInstance> SagaInstances => Set<SagaInstance>();
    public DbSet<SagaStep> SagaSteps => Set<SagaStep>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SagaInstance>(entity =>
        {
            entity.ToTable("SagaInstance");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.CorrelationId)
                .HasColumnName("CorrelationId")
                .IsRequired();

            entity.Property(x => x.SagaType)
                .HasColumnName("SagaType")
                .HasConversion<int>()
                .IsRequired();

            entity.Property(x => x.Status)
                .HasColumnName("Status")
                .HasConversion<int>()
                .IsRequired();

            entity.Property(x => x.CurrentStep)
                .HasColumnName("CurrentStep")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.ErrorMessage)
                .HasColumnName("ErrorMessage")
                .HasMaxLength(2000);

            entity.Property(x => x.CreatedAtUtc)
                .HasColumnName("CreatedAtUtc")
                .IsRequired();

            entity.Property(x => x.UpdatedAtUtc)
                .HasColumnName("UpdatedAtUtc")
                .IsRequired();

            entity.HasMany(x => x.Steps)
                .WithOne()
                .HasForeignKey(x => x.SagaInstanceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SagaStep>(entity =>
        {
            entity.ToTable("SagaStep");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.SagaInstanceId)
                .HasColumnName("SagaInstanceId")
                .IsRequired();

            entity.Property(x => x.StepName)
                .HasColumnName("StepName")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Status)
                .HasColumnName("Status")
                .HasConversion<int>()
                .IsRequired();

            entity.Property(x => x.RequestPayload)
                .HasColumnName("RequestPayload");

            entity.Property(x => x.ResponsePayload)
                .HasColumnName("ResponsePayload");

            entity.Property(x => x.ErrorMessage)
                .HasColumnName("ErrorMessage")
                .HasMaxLength(2000);

            entity.Property(x => x.IsCompensation)
                .HasColumnName("IsCompensation")
                .IsRequired();

            entity.Property(x => x.CreatedAtUtc)
                .HasColumnName("CreatedAtUtc")
                .IsRequired();
        });
    }
}
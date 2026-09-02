using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Infrastructure.Data.Configurations;

public sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnType("uuid").HasDefaultValueSql("uuidv7()");
        builder.ToTable("reservations");

        builder.Property(e => e.ExpiresAt).IsRequired().HasColumnType("timestamp with time zone");

        builder
            .HasOne(r => r.Order)
            .WithMany(o => o.Reservations)
            .HasForeignKey(r => r.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(r => r.Product)
            .WithMany()
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(r => r.Warehouse)
            .WithMany()
            .HasForeignKey(r => r.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
    }
}

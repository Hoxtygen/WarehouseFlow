using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WarehouseFlow.Infrastructure.Data.Configurations;

public sealed class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnType("uuid").HasDefaultValueSql("uuidv7()");
        builder.HasIndex(i => new { i.ProductId, i.WarehouseId }).IsUnique();
        builder.ToTable("inventories");

        builder.Property(e => e.AvailableQuantity).IsRequired();
        builder.Property(e => e.ReservedQuantity).IsRequired();
        builder.Property(e => e.LastReservedAt).HasColumnType("timestamp with time zone");

        builder.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

        builder
            .HasOne(i => i.Product)
            .WithMany(p => p.Inventories)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(i => i.Warehouse)
            .WithMany(w => w.Inventories)
            .HasForeignKey(i => i.WarehouseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

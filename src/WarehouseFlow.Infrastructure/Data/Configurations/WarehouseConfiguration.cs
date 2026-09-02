using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WarehouseFlow.Infrastructure.Data.Configurations;

public sealed class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnType("uuid").HasDefaultValueSql("uuidv7()");
        builder.ToTable("warehouses");
        builder.HasIndex(e => e.WarehouseCode).IsUnique();
        builder.Property(e => e.WarehouseName).IsRequired();
        builder.Property(e => e.Capacity).IsRequired();
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Infrastructure.Data.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnType("uuid").HasDefaultValueSql("uuidv7()");
        builder.ToTable("products");
        builder.HasIndex(e => e.SKU).IsUnique();
        builder.Property(e => e.ProductName).IsRequired();
        builder.Property(e => e.UnitPrice).IsRequired();
        builder.Property(e => e.Description).IsRequired();
        builder.Property(e => e.Brand);
        builder.Property(e => e.ImageUrl);
        builder.Property(e => e.ProductName).IsRequired();
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

        builder
            .HasOne(product => product.ProductCategory)
            .WithMany(category => category.Products)
            .HasForeignKey(product => product.ProductCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

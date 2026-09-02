using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Infrastructure.Data.Configurations;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnType("uuid").HasDefaultValueSql("uuidv7()");
        builder.ToTable("payments");

        builder.Property(e => e.Amount).IsRequired();

        builder
            .HasOne<Order>()
            .WithMany()
            .HasForeignKey(payment => payment.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne<Customer>()
            .WithMany()
            .HasForeignKey(payment => payment.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
    }
}

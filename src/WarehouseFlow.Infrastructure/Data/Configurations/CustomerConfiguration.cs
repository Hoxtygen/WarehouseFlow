using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Infrastructure.Data.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnType("uuid").HasDefaultValueSql("uuidv7()");
        builder.ToTable("Customers");
        builder.HasIndex(e => e.Email).IsUnique();
        builder.HasIndex(e => e.PhoneNumber).IsUnique();

        builder.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.LastName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(150);
        builder.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(14);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

        builder.OwnsOne(
            e => e.Address,
            address =>
            {
                address.Property(a => a.Street).IsRequired().HasMaxLength(200);
                address.Property(a => a.HouseNumber).IsRequired().HasMaxLength(5);
                address.Property(a => a.City).IsRequired().HasMaxLength(100);
                address.Property(a => a.State).IsRequired().HasMaxLength(100);
                address.Property(a => a.LandMark).HasMaxLength(200);
            }
        );
        builder
            .HasOne<ApplicationUser>()
            .WithOne(user => user.Customer)
            .HasForeignKey<Customer>(customer => customer.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

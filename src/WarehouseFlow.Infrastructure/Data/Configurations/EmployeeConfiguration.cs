using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Infrastructure.Data.Configurations;

public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnType("uuid").HasDefaultValueSql("uuidv7()");
        builder.ToTable("employees");
        builder.HasIndex(e => e.EmployeeNumber).IsUnique();
        builder.Property(e => e.EmployeeNumber).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Address).IsRequired().HasMaxLength(250);
        builder.Property(e => e.Role).HasConversion<string>().IsRequired().HasMaxLength(50);
        builder.Property(e => e.CreatedByUserId).IsRequired();
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

        builder
            .HasOne<ApplicationUser>()
            .WithOne(user => user.Employee)
            .HasForeignKey<Employee>(employee => employee.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(employee => employee.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

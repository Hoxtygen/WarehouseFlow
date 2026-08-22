using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>().ToTable("users");
        modelBuilder.Entity<ApplicationUser>().HasIndex(user => user.PhoneNumber).IsUnique();
        modelBuilder.Entity<IdentityRole>().ToTable("roles");

        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("user_roles");

        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("user_claims");

        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("role_claims");

        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("user_logins");

        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("user_tokens");

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("uuid").HasDefaultValueSql("uuidv7()");
            entity.ToTable("Customers");
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.PhoneNumber).IsUnique();

            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(14);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.OwnsOne(
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
            entity
                .HasOne<ApplicationUser>()
                .WithOne(user => user.Customer)
                .HasForeignKey<Customer>(customer => customer.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("uuid").HasDefaultValueSql("uuidv7()");
            entity.ToTable("employees");
            entity.HasIndex(e => e.EmployeeNumber).IsUnique();
            entity.Property(e => e.EmployeeNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(250);
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CreatedByUserId).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity
                .HasOne<ApplicationUser>()
                .WithOne(user => user.Employee)
                .HasForeignKey<Employee>(employee => employee.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(employee => employee.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("uuid").HasDefaultValueSql("uuidv7()");
            entity.ToTable("warehouses");
            entity.HasIndex(e => e.WarehouseCode).IsUnique();
            entity.Property(e => e.WarehouseName).IsRequired();
            entity.Property(e => e.Capacity).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("uuid").HasDefaultValueSql("uuidv7()");
            entity.ToTable("product_categories");
            entity.HasIndex(e => e.CategoryName);
            entity.HasIndex(e => e.CategorySlug);
            entity.Property(e => e.Description);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("uuid").HasDefaultValueSql("uuidv7()");
            entity.ToTable("products");
            entity.HasIndex(e => e.SKU).IsUnique();
            entity.Property(e => e.ProductName).IsRequired();
            entity.Property(e => e.UnitPrice).IsRequired();
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Brand);
            entity.Property(e => e.ImageUrl);
            entity.Property(e => e.ProductName).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity
                .HasOne(product => product.ProductCategory)
                .WithMany(category => category.Products)
                .HasForeignKey(product => product.ProductCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("uuid").HasDefaultValueSql("uuidv7()");
            entity.ToTable("inventories");

            entity.Property(e => e.AvailableQuantity).IsRequired();
            entity.Property(e => e.ReservedQuantity).IsRequired();
            entity.Property(e => e.LastReservedAt).HasColumnType("timestamp with time zone");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity
                .HasOne(i => i.Product)
                .WithMany(p => p.Inventories)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(i => i.Warehouse)
                .WithMany(w => w.Inventories)
                .HasForeignKey(i => i.WarehouseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("uuid").HasDefaultValueSql("uuidv7()");
            entity.ToTable("orders");

            entity.Property(e => e.TotalAmount).IsRequired();
            entity.Property(e => e.OrderDate).HasDefaultValueSql("now()");
            entity.Property(e => e.OrderStatus).HasConversion<string>().IsRequired();

            entity
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("uuid").HasDefaultValueSql("uuidv7()");
            entity.ToTable("order_items");

            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.UnitPrice).IsRequired();

            entity
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("uuid").HasDefaultValueSql("uuidv7()");
            entity.ToTable("reservations");

            entity.Property(e => e.ReservedQuantity).IsRequired();
            entity
                .Property(e => e.ExpiresAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            entity
                .HasOne(r => r.Order)
                .WithMany(o => o.Reservations)
                .HasForeignKey(r => r.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(r => r.Product)
                .WithMany()
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(r => r.Warehouse)
                .WithMany()
                .HasForeignKey(r => r.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
        });
    }
}

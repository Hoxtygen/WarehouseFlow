using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Domain.Entities;
using WarehouseFlow.Infrastructure.Data.Interceptors;

namespace WarehouseFlow.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    private readonly AuditInterceptor _auditInterceptor;

    public AppDbContext(DbContextOptions<AppDbContext> options, AuditInterceptor auditInterceptor)
        : base(options)
    {
        _auditInterceptor = auditInterceptor;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditInterceptor);
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

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

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}

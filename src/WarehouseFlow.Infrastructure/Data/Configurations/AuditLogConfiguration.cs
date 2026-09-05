using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Infrastructure.Data.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
               .HasColumnType("uuid")
               .HasDefaultValueSql("uuidv7()");

        builder.ToTable("audit_logs");

        builder.Property(e => e.UserId)
               .IsRequired()
               .HasMaxLength(450);   // matches ASP.NET Identity key length

        builder.Property(e => e.Action)
               .IsRequired()
               .HasMaxLength(20);    // "Added" | "Modified" | "Deleted"

        builder.Property(e => e.EntityName)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(e => e.EntityId)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(e => e.ChangesJson)
               .HasColumnType("jsonb");   // PostgreSQL native JSON — enables GIN indexing later

        builder.Property(e => e.IpAddress)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(e => e.CreatedAt)
               .HasDefaultValueSql("now()");

        // UpdatedAt is intentionally not set on audit logs — they are immutable.
        builder.Property(e => e.UpdatedAt)
               .IsRequired(false);

        // Common query: "show me all audit records for a given entity row"
        builder.HasIndex(e => new { e.EntityName, e.EntityId });

        // Common query: "what did user X do?"
        builder.HasIndex(e => e.UserId);
    }
}

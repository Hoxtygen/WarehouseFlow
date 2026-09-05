using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Infrastructure.Data.Interceptors;

/// <summary>
/// SaveChangesInterceptor that automatically writes <see cref="AuditLog"/> records
/// for every tracked entity state change (Added / Modified / Deleted) in the same
/// database transaction as the originating SaveChanges call.
///
/// Design notes:
/// - Entries are snapshotted <em>before</em> EF flushes so original values are still readable.
/// - AuditLog rows are added to the same context so they are saved in the same transaction.
/// - The AuditLog table itself is excluded to prevent infinite recursion.
/// - Both the sync and async SaveChanges paths are covered.
/// </summary>
public sealed class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // ------------------------------------------------------------------
    // Async path  (called by SaveChangesAsync)
    // ------------------------------------------------------------------

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            WriteAuditLogs(eventData.Context);          // synchronous snapshot + Add

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    // ------------------------------------------------------------------
    // Sync path  (called by SaveChanges)
    // ------------------------------------------------------------------

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
            WriteAuditLogs(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    // ------------------------------------------------------------------
    // Core logic — must be synchronous so it runs safely in both paths
    // ------------------------------------------------------------------

    private void WriteAuditLogs(DbContext context)
    {
        // Resolve caller identity; fall back to "system" for background jobs / migrations.
        var userId = _httpContextAccessor.HttpContext?
            .User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? "system";

        var ipAddress = GetClientIp();

        // Materialise the dirty-entry list NOW, before EF shifts internal state.
        var entries = context.ChangeTracker
            .Entries()
            .Where(static e =>
                e.Entity is not AuditLog &&   // never audit the audit table itself
                e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        if (entries.Count == 0)
            return;

        var logs = new List<AuditLog>(entries.Count);

        foreach (var entry in entries)
        {
            var log = BuildAuditLog(entry, userId, ipAddress);
            if (log is not null)
                logs.Add(log);
        }

        if (logs.Count > 0)
            context.Set<AuditLog>().AddRange(logs);
    }

    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------

    private static AuditLog? BuildAuditLog(EntityEntry entry, string userId, string ipAddress)
    {
        var entityName = entry.Entity.GetType().Name;
        var entityId   = GetPrimaryKeyValue(entry);
        var action     = entry.State.ToString();   // "Added" | "Modified" | "Deleted"

        // Build the changes payload depending on the operation type.
        Dictionary<string, object?> changes = entry.State switch
        {
            // Added: capture every property's new value.
            EntityState.Added =>
                entry.CurrentValues.Properties
                     .ToDictionary(p => p.Name, p => entry.CurrentValues[p]),

            // Deleted: preserve what was there before it was removed.
            EntityState.Deleted =>
                entry.OriginalValues.Properties
                     .ToDictionary(p => p.Name, p => entry.OriginalValues[p]),

            // Modified: only properties that actually changed, as { From, To } pairs.
            EntityState.Modified =>
                entry.Properties
                     .Where(static p => p.IsModified)
                     .ToDictionary(
                         p => p.Metadata.Name,
                         p => (object?)new { From = p.OriginalValue, To = p.CurrentValue }),

            _ => []
        };

        // Omit entries that produced no recordable changes (can happen with
        // touch-only SaveChanges calls on Modified entities where no scalar
        // properties actually differ).
        if (changes.Count == 0 && entry.State == EntityState.Modified)
            return null;

        return new AuditLog
        {
            UserId      = userId,
            Action      = action,
            EntityName  = entityName,
            EntityId    = entityId,
            ChangesJson = changes.Count > 0
                ? JsonSerializer.Serialize(changes, JsonOptions)
                : null,
            IpAddress   = ipAddress
        };
    }

    private static string GetPrimaryKeyValue(EntityEntry entry)
    {
        var keyValues = entry.Metadata
            .FindPrimaryKey()
            ?.Properties
            .Select(p => entry.Property(p.Name).CurrentValue?.ToString() ?? "null")
            .ToArray();

        return keyValues is { Length: > 0 }
            ? string.Join("|", keyValues)
            : "unknown";
    }

    private string GetClientIp()
    {
        var ctx = _httpContextAccessor.HttpContext;
        if (ctx is null) return "unknown";

        // Honour X-Forwarded-For when running behind a reverse proxy / load balancer.
        var forwarded = ctx.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwarded))
            return forwarded.Split(',')[0].Trim();

        return ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented          = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };
}

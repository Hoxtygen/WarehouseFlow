namespace WarehouseFlow.Domain.Entities;


public class AuditLog:BaseEntity
{
    public required string UserId { get; set; }
    public required string Action { get; set; }
    public required string EntityName { get; set; }
    public required string EntityId { get; set; }
    public string? ChangesJson { get; set; }
    public required string IpAddress { get; set; }
}
using WarehouseFlow.Domain.Entities;

public class Warehouse : BaseEntity
{
    public string WarehouseName { get; set; } = string.Empty;
    public string WarehouseCode { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Capacity { get; set; }

    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}

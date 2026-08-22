using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Domain.Entities;
public class Product : BaseEntity
{
    public required string ProductName { get; set; } = string.Empty;
    public required decimal UnitPrice { get; set; }
    public required  Guid ProductCategoryId { get; set; }
    public ProductCategory ProductCategory {get; set;}= null!;
    public required string SKU { get; set; } = string.Empty;
    public required string Description { get; set; } = string.Empty;
    public string? Brand { get; set; }  = "Generic";
    public bool IsActive { get; set; } = true;
    public string? ImageUrl { get; set; }
    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}

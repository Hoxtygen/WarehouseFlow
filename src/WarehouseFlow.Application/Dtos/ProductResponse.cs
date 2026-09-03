using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Application.Dtos;

public class ProductResponse
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public required string Brand { get; set; } = string.Empty;
    public required string SKU { get; set; } = string.Empty;
    public required ProductCategory ProductCategory { get; set; }
}

public static class ProductResponseFactory
{
    public static ProductResponse FromProduct(Product product) =>
        new()
        {
            Id = product.Id,
            ProductName = product.ProductName,
            Description = product.Description,
            UnitPrice = product.UnitPrice,
            Brand = product.Brand,
            ProductCategory = product.ProductCategory,
            SKU = product.SKU,
        };
}

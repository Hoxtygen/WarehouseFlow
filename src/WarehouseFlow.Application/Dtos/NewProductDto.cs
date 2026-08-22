using System.ComponentModel.DataAnnotations;
using WarehouseFlow.Domain.Enum;

namespace WarehouseFlow.Application.Dtos;

public class NewProductDto
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(
        50,
        ErrorMessage = "Product name must be betweeen  and  50 characters",
        MinimumLength = 5
    )]
    public string ProductName { get; set; } = string.Empty;

    [Range(0.01, 999999999.99, ErrorMessage = "Unit price must be greater than 0")]
    public decimal UnitPrice { get; set; }

    [Required(ErrorMessage = "Product category is required")]
    public Guid ProductCategoryId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Brand { get; set; } = "Generic";
    public string? ImageUrl { get; set; }
}

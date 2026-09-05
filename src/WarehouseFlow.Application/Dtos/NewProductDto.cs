using System.ComponentModel.DataAnnotations;
using WarehouseFlow.Domain.Enum;

namespace WarehouseFlow.Application.Dtos;

public record NewProductDto(
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(
        50,
        ErrorMessage = "Product name must be betweeen  and  50 characters",
        MinimumLength = 5
    )]
        string ProductName,
    [Range(0.01, 999999999.99, ErrorMessage = "Unit price must be greater than 0")]
        decimal UnitPrice,
    [Required(ErrorMessage = "Product category is required")] Guid ProductCategoryId,
    [Required, StringLength(1000)] string Description,
    [StringLength(50)] string Brand,
    [Url, StringLength(500)] string? ImageUrl
);

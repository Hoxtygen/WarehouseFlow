using System.ComponentModel.DataAnnotations;

namespace WarehouseFlow.Application.Dtos
{
    public record OrderItemDto(
        Guid productId,
        [Range(1, 999, ErrorMessage = "Quantity must be greater than 0")] int Quantity,
        [Range(1, 999999999, ErrorMessage = "Price must be greater than 0")]
            decimal UnitPrice
    );
}

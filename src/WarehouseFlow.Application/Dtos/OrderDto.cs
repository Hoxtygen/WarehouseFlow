using System.ComponentModel.DataAnnotations;

namespace WarehouseFlow.Application.Dtos
{
    public record OrderDto(
        [MinLength(1, ErrorMessage = "You must order for at least one item")]
            List<OrderItemDto> OrderItems
    );
}

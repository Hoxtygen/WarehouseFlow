using System.ComponentModel.DataAnnotations;

namespace WarehouseFlow.Application.Dtos
{
 using System.ComponentModel.DataAnnotations;

namespace WarehouseFlow.Application.Dtos
{
    public record InventoryDto(
        Guid ProductId,

        [Range(
            1,
            999999999,
            ErrorMessage = "Available quantity must be greater than 0"
        )]
        int AvailableQuantity,

        [Range(
            0,
            999999999,
            ErrorMessage = "Reserved quantity must be greater than or equal to 0"
        )]
        int ReservedQuantity,

        Guid WarehouseId
    );
}

}

namespace WarehouseFlow.Application.Dtos;

public record InventoryReservationDto(Guid WarehouseId, int ReservedQuantity);

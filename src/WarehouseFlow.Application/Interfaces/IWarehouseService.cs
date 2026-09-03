using WarehouseFlow.Application.Dtos;

namespace WarehouseFlow.Application.Interfaces;

public interface IWarehouseService
{
    Task<WarehouseResponse> CreateWarehouse(
        NewWarehouseDto newWarehouseDto,
        CancellationToken cancellationToken = default
    );
    Task<bool> WarehouseExists(Guid warehouseId, CancellationToken cancellationToken);
}

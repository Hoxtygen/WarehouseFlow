
using WarehouseFlow.Application.Dtos;

namespace WarehouseFlow.Application.Interfaces;


public interface IWarehouseService
{
    Task<Warehouse> CreateWarehouse(NewWarehouseDto newWarehouseDto, CancellationToken cancellationToken = default);
}
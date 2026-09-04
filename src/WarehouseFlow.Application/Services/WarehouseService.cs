using Microsoft.Extensions.Logging;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Application.Interfaces;
using WarehouseFlow.Domain.Services;

namespace WarehouseFlow.Application.Services;

public sealed class WarehouseService(
    IWarehouseRepository warehouseRepository,
    IUnitOfWork unitOfWork,
    ILogger<WarehouseService> logger
) : IWarehouseService
{
    public async Task<WarehouseResponse> CreateWarehouse(
        NewWarehouseDto newWarehouseDto,
        CancellationToken cancellationToken
    )
    {
        var newWarehouse = new Warehouse
        {
            WarehouseName = newWarehouseDto.WarehouseName,
            Address = newWarehouseDto.Address,
            Capacity = newWarehouseDto.Capacity,
            WarehouseCode = WarehouseCodeGenerator.Generate(newWarehouseDto.Location),
        };

        await warehouseRepository.AddAsync(newWarehouse, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Warehouse {WarehouseName} with code {WarehouseCode} created successfully",
            newWarehouse.WarehouseName,
            newWarehouse.WarehouseCode
        );

        return WarehouseResponseFactory.FromWarehouse(newWarehouse);
    }

    public async Task<bool> WarehouseExists(Guid warehouseId, CancellationToken cancellationToken)
    {
        return await warehouseRepository.ExistsAsync(warehouseId, cancellationToken);
    }

    public async Task<int> GetWarehouseCapacity(
        Guid warehouseId,
        CancellationToken cancellationToken
    )
    {
        return await warehouseRepository.GetCapacityAsync(warehouseId, cancellationToken);
    }
}

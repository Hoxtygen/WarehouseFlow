using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Application.Interfaces;
using WarehouseFlow.Domain.Services;
using WarehouseFlow.Infrastructure.Data;

namespace WarehouseFlow.Infrastructure.Implementations
{
    public class WarehouseService(AppDbContext dbContext, ILogger<WarehouseService> _logger)
        : IWarehouseService
    {
        public async Task<Warehouse> CreateWarehouse(
            NewWarehouseDto newWarehouseDto,
            CancellationToken cancellationToken
        )
        {
            var newWarehouse = new Warehouse()
            {
                WarehouseName = newWarehouseDto.WarehouseName,
                Address = newWarehouseDto.Address,
                Capacity = newWarehouseDto.Capacity,
                WarehouseCode = WarehouseCodeGenerator.Generate(newWarehouseDto.Location),
            };

            await dbContext.AddAsync(newWarehouse);
            await dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation(
                "Warehouse {WarehouseName} with code {WarehouseCode} created successfully",
                newWarehouse.WarehouseName,
                newWarehouse.WarehouseCode
            );
            return newWarehouse;
        }

           public async Task<bool> WarehouseExists(
            Guid warehouseId,
            CancellationToken cancellationToken
        )
        {
            return await dbContext.Warehouses.AnyAsync(w => w.Id == warehouseId, cancellationToken);
        }
    }
}

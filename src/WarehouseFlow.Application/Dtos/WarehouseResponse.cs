namespace WarehouseFlow.Application.Dtos;

public class WarehouseResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public int Capacity { get; set; }
    
}

public static class WarehouseResponseFactory
{
    public static WarehouseResponse FromWarehouse(Warehouse warehouse) => new()
    {
        Id = warehouse.Id,
        Name = warehouse.WarehouseName,
        Address = warehouse.Address,
        Capacity = warehouse.Capacity
    };
}
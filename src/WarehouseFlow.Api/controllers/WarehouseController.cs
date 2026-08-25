using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseFlow.Api.Contracts;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Application.Interfaces;

namespace WarehouseFlow.Api.Controllers;

[Route("api/v1/warehouse")]
public class WarehouseController(IWarehouseService warehouseService) : BaseController
{
    [HttpPost("createWarehouse")]
     [Authorize(Roles = "Super_Admin, Admin")]
    [ProducesResponseType(typeof(ApiResponse<Warehouse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateWareHouse(
        NewWarehouseDto request,
        CancellationToken cancellationToken
    )
    {
        var result = await warehouseService.CreateWarehouse(request, cancellationToken);
        return Created(
            result,
            nameof(CreateWareHouse),
            new { id = result.Id },
            "Warehouse created successfully."
        );
    }
}

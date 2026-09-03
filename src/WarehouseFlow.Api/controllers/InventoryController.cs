using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseFlow.Api.Contracts;
using WarehouseFlow.Api.Controllers;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Application.Interfaces;

namespace WarehouseFlow.Api.Controllers
{
    [Route("api/v1/inventories")]
    public sealed class InventoryController(IInventoryService inventoryService) : BaseController
    {
        [HttpPost]
        [Authorize(Roles = "Super_Admin, Admin, Warehouse_Manager")]
        [ProducesResponseType(typeof(ApiResponse<InventoryResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateInventory(
            InventoryDto request,
            CancellationToken cancellationToken
        )
        {
            var result = await inventoryService.CreateInventory(request, cancellationToken);
            return Created(
                result,
                nameof(CreateInventory),
                new { id = result.Id },
                "New inventory added successfully."
            );
        }
    }
}

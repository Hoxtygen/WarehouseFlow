using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseFlow.Api.Contracts;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Application.Interfaces;
using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Api.Controllers;
[Route("api/v1/product")]
public sealed class ProductController(IProductService productService) : BaseController
{
    [HttpPost("createProduct")]
    [Authorize(Roles = "Super_Admin, Admin")]
    [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> createProduct(
        NewProductDto request,
        CancellationToken cancellationToken
    )
    {
        var result = await productService.createProduct(request, cancellationToken);
        return Created(
            result,
            nameof(createProduct),
            new { id = result.Id },
            "New product added successfully."
        );
    }
}

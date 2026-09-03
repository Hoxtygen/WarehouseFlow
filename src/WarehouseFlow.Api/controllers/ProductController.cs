using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseFlow.Api.Contracts;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Application.Interfaces;
using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Api.Controllers;
[Route("api/v1/products")]
public sealed class ProductController(IProductService productService) : BaseController
{
    [HttpPost]
    [Authorize(Roles = "Super_Admin, Admin")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct(
        NewProductDto request,
        CancellationToken cancellationToken
    )
    {
        var result = await productService.createProduct(request, cancellationToken);
        return Created(
            result,
            nameof(CreateProduct),
            new { id = result.Id },
            "New product added successfully."
        );
    }
}

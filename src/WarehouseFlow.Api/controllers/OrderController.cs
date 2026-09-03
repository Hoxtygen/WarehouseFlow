using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WarehouseFlow.Api.Contracts;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Application.Interfaces;

namespace WarehouseFlow.Api.Controllers;

[Route("api/v1/orders")]
public class OrderController(IOrderService orderService) : BaseController
{
    [HttpPost]
    [EnableRateLimiting("orders")]
    [Authorize(Roles = "Customer")]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateOrder(
        [FromBody] OrderDto request,
        CancellationToken cancellationToken
    )
    {
        var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(applicationUserId))
        {
            return Problem(
                title: "The authenticated user ID is missing from the access token.",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        var order = await orderService.CreateOrder(request, applicationUserId, cancellationToken);
        return Created(
            order,
            nameof(CreateOrder),
            new { id = order.Id },
            "Order created successfully"
        );
    }
}

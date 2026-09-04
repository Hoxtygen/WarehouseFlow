using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WarehouseFlow.Api.Controllers;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Application.Interfaces;

namespace WarehouseFlow.Api.Contracts;

[Route("api/v1/payment")]
public sealed class PaymentController(IPaymentService paymentService) : BaseController
{
    [HttpPost("processPayment")]
    [EnableRateLimiting("orders")]
    [Authorize(Roles = "Customer")]
    [ProducesResponseType(typeof(ApiResponse<PaymentResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ProcessPayment(
        [FromBody] PaymentDto request,
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
        var result = await paymentService.ProcessPaymentAsync(request, applicationUserId, cancellationToken);
        return Created(
            result,
            nameof(ProcessPayment),
            new { id = result.paymentId },
            "Order created successfully"
        );
    }
}

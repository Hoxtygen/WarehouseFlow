

using Microsoft.AspNetCore.Mvc;

namespace WarehouseFlow.Api.Controllers;

[ApiController]
[Route("api")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("WarehouseFlow API is running.");
    }
}
using Microsoft.AspNetCore.Mvc;
using SmartCart.Backend.Services;

namespace SmartCart.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("{orderId:guid}")]
    public IActionResult GetOrder(Guid orderId)
    {
        var order = _orderService.GetOrder(orderId);
        if (order == null)
            return NotFound();

        return Ok(order);
    }
}

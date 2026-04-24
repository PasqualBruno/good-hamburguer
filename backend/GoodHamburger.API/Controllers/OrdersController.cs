using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateOrderRequest request)
    {
        var order = _orderService.CreateOrder(request);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        var order = _orderService.GetOrderById(id);
        return Ok(order);
    }

    [HttpGet("active")]
    public IActionResult GetActive()
    {
        var orders = _orderService.GetActiveOrders();
        return Ok(orders);
    }
}

using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.API.Controllers;

[ApiController]
[Route("api/admin/orders")]
[Authorize(Roles = "Admin")]
public class AdminOrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public AdminOrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var orders = _orderService.GetAllOrders();
        return Ok(orders);
    }

    [HttpPatch("{id}/status")]
    public IActionResult UpdateStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
    {
        var order = _orderService.UpdateStatus(id, request.Status);
        return Ok(order);
    }
}

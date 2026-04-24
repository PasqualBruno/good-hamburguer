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

    /// <summary>
    /// Cria um novo pedido completo.
    /// Valida itens, aplica promoção automaticamente, calcula totais e salva.
    /// Retorna 201 com o pedido criado ou 400 com código de erro.
    /// </summary>
    [HttpPost]
    public IActionResult Create([FromBody] CreateOrderRequest request)
    {
        var order = _orderService.CreateOrder(request);
        return CreatedAtAction(nameof(Create), new { id = order.Id }, order);
    }
}

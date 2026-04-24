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
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    /// <summary>
    /// Busca um pedido pelo ID.
    /// Usado pelo cliente para acompanhar o status (Follow Order).
    /// </summary>
    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        var order = _orderService.GetOrderById(id);
        return Ok(order);
    }

    /// <summary>
    /// Lista pedidos ativos (Não entregues/cancelados).
    /// Usado pelo Telão (Display Board).
    /// </summary>
    [HttpGet("active")]
    public IActionResult GetActive()
    {
        var orders = _orderService.GetActiveOrders();
        return Ok(orders);
    }
}

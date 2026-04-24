using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Hubs;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using GoodHamburger.Domain.Errors;
using GoodHamburger.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace GoodHamburger.Application.Services;

public class OrderService : IOrderService
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IPromotionService _promotionService;
    private readonly IHubContext<OrderHub> _hubContext;

    public OrderService(
        IMenuItemRepository menuItemRepository,
        IOrderRepository orderRepository,
        IPromotionService promotionService,
        IHubContext<OrderHub> hubContext)
    {
        _menuItemRepository = menuItemRepository;
        _orderRepository = orderRepository;
        _promotionService = promotionService;
        _hubContext = hubContext;
    }

    public OrderResponse CreateOrder(CreateOrderRequest request)
    {
        // 1. Validar se lista não está vazia
        if (request.MenuItemIds == null || request.MenuItemIds.Count == 0)
        {
            throw new DomainError(
                DomainErrorCodes.EmptyOrder,
                "O pedido deve conter pelo menos 1 item.");
        }

        // 2. Buscar os MenuItems pelos IDs
        var menuItems = new List<MenuItem>();
        foreach (var id in request.MenuItemIds)
        {
            var item = _menuItemRepository.GetById(id);
            if (item == null)
            {
                throw new DomainError(
                    DomainErrorCodes.InvalidMenuItem,
                    $"Item do cardápio não encontrado: {id}.");
            }
            menuItems.Add(item);
        }

        // 3. Validar duplicatas por tipo (máximo 1 de cada)
        ValidateNoDuplicateTypes(menuItems);

        // 4. Criar OrderItems a partir dos MenuItems
        var orderItems = menuItems.Select(item => new OrderItem(
            menuItemId: item.Id,
            menuItemName: item.Name,
            unitPrice: item.Price,
            type: item.Type
        )).ToList();

        // 5. Encontrar a melhor promoção aplicável
        var itemTypes = menuItems.Select(i => i.Type);
        var promotion = _promotionService.FindBestPromotion(itemTypes);

        // 6. Criar o pedido (calcula subtotal, desconto, total)
        var order = Order.Create(orderItems, promotion);

        // 7. Salvar o pedido
        _orderRepository.Add(order);

        // 8. Notificar restaurante via SignalR
        var response = MapToResponse(order);
        _hubContext.Clients.Group("admin").SendAsync("NewOrderReceived", response);

        // 9. Retornar response
        return response;
    }

    public OrderResponse UpdateStatus(Guid orderId, string newStatusStr)
    {
        // 1. Buscar pedido
        var order = _orderRepository.GetById(orderId);
        if (order == null)
        {
            throw new DomainError(DomainErrorCodes.OrderNotFound, "Pedido não encontrado.");
        }

        // 2. Tentar converter string para enum
        if (!Enum.TryParse<OrderStatus>(newStatusStr, true, out var newStatus))
        {
            throw new DomainError(DomainErrorCodes.InvalidStatusTransition, $"Status inválido: {newStatusStr}");
        }

        var oldStatus = order.Status;

        // 3. Alterar status (validação ocorre dentro da entidade)
        order.ChangeStatus(newStatus);

        // 4. Salvar
        _orderRepository.Update(order);

        // 5. Notificar via SignalR
        _hubContext.Clients.Group("display").SendAsync("OrderStatusChanged", new
        {
            orderId = order.Id,
            code = order.Code,
            oldStatus = oldStatus.ToString(),
            newStatus = order.Status.ToString()
        });

        _hubContext.Clients.Group("admin").SendAsync("OrderStatusChanged", new
        {
            orderId = order.Id,
            code = order.Code,
            oldStatus = oldStatus.ToString(),
            newStatus = order.Status.ToString()
        });

        // Notificar grupo específico do pedido (cliente)
        _hubContext.Clients.Group($"order-{order.Id}").SendAsync("OrderStatusChanged", new
        {
            orderId = order.Id,
            code = order.Code,
            newStatus = order.Status.ToString()
        });

        return MapToResponse(order);
    }

    public List<OrderResponse> GetAllOrders()
    {
        return _orderRepository.GetAll()
            .Select(MapToResponse)
            .OrderByDescending(o => o.CreatedAt)
            .ToList();
    }

    public OrderResponse GetOrderById(Guid id)
    {
        var order = _orderRepository.GetById(id);
        if (order == null)
        {
            throw new DomainError(DomainErrorCodes.OrderNotFound, "Pedido não encontrado.");
        }
        return MapToResponse(order);
    }

    public List<OrderResponse> GetActiveOrders()
    {
        // Retorna pedidos que ainda não foram entregues nem cancelados
        return _orderRepository.GetAll()
            .Where(o => o.Status != OrderStatus.Delivered && o.Status != OrderStatus.Cancelled)
            .Select(MapToResponse)
            .OrderBy(o => o.CreatedAt)
            .ToList();
    }

    /// <summary>
    /// Valida que não há mais de 1 item do mesmo tipo no pedido.
    /// </summary>
    private static void ValidateNoDuplicateTypes(List<MenuItem> items)
    {
        var typeCounts = items.GroupBy(i => i.Type)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        foreach (var type in typeCounts)
        {
            var (code, message) = type switch
            {
                MenuItemType.Sandwich => (
                    DomainErrorCodes.DuplicateSandwich,
                    "Só é permitido 1 sanduíche por pedido."),
                MenuItemType.Side => (
                    DomainErrorCodes.DuplicateSide,
                    "Só é permitido 1 acompanhamento por pedido."),
                MenuItemType.Drink => (
                    DomainErrorCodes.DuplicateDrink,
                    "Só é permitida 1 bebida por pedido."),
                _ => ("UNKNOWN", "Erro desconhecido.")
            };

            throw new DomainError(code, message);
        }
    }

    private static OrderResponse MapToResponse(Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            Code = order.Code,
            Items = order.Items.Select(i => new OrderItemDto
            {
                MenuItemId = i.MenuItemId,
                Name = i.MenuItemName,
                Price = i.UnitPrice,
                Type = i.Type.ToString()
            }).ToList(),
            Subtotal = order.Subtotal,
            PromotionName = order.PromotionName,
            DiscountPercent = order.DiscountPercent * 100, // 0.20 → 20
            DiscountValue = order.DiscountValue,
            Total = order.Total,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt
        };
    }
}

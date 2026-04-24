using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using GoodHamburger.Domain.Errors;
using GoodHamburger.Domain.Interfaces;

namespace GoodHamburger.Application.Services;

public class OrderService : IOrderService
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IPromotionService _promotionService;

    public OrderService(
        IMenuItemRepository menuItemRepository,
        IOrderRepository orderRepository,
        IPromotionService promotionService)
    {
        _menuItemRepository = menuItemRepository;
        _orderRepository = orderRepository;
        _promotionService = promotionService;
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

        // 8. Retornar response
        return MapToResponse(order);
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

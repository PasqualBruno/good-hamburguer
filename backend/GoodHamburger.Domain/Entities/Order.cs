using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public List<OrderItem> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public string? PromotionName { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal Total { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public Order() { }

    /// <summary>
    /// Cria um novo pedido com cálculos de subtotal, desconto e total.
    /// </summary>
    public static Order Create(List<OrderItem> items, Promotion? promotion)
    {
        var subtotal = items.Sum(i => i.UnitPrice);
        var discountPercent = promotion?.DiscountPercent ?? 0m;
        var discountValue = Math.Round(subtotal * discountPercent, 2);
        var total = subtotal - discountValue;

        return new Order
        {
            Id = Guid.NewGuid(),
            Code = GenerateCode(),
            Items = items,
            Subtotal = subtotal,
            PromotionName = promotion?.Name,
            DiscountPercent = discountPercent,
            DiscountValue = discountValue,
            Total = total,
            Status = OrderStatus.Received,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Altera o status do pedido com validação de transições permitidas.
    /// </summary>
    public void ChangeStatus(OrderStatus newStatus)
    {
        if (Status == newStatus) return;

        bool isValid = Status switch
        {
            OrderStatus.Received => newStatus == OrderStatus.Preparing || newStatus == OrderStatus.Cancelled,
            OrderStatus.Preparing => newStatus == OrderStatus.Ready || newStatus == OrderStatus.Cancelled,
            OrderStatus.Ready => newStatus == OrderStatus.Delivered,
            OrderStatus.Delivered => false,
            OrderStatus.Cancelled => false,
            _ => false
        };

        if (!isValid)
        {
            throw new GoodHamburger.Domain.Errors.DomainError(
                GoodHamburger.Domain.Errors.DomainErrorCodes.InvalidStatusTransition,
                $"Transição de status inválida: {Status} para {newStatus}.");
        }

        Status = newStatus;
    }

    /// <summary>
    /// Gera um código curto amigável para exibição (ex: "#A42").
    /// </summary>
    private static string GenerateCode()
    {
        var random = new Random();
        var letter = (char)('A' + random.Next(0, 26));
        var number = random.Next(10, 99);
        return $"#{letter}{number}";
    }
}

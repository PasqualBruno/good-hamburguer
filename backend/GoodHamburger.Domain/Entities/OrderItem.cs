using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Domain.Entities;

public class OrderItem
{
    public int Id { get; set; }
    public int MenuItemId { get; set; }
    public string MenuItemName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public MenuItemType Type { get; set; }

    public OrderItem() { }

    public OrderItem(int menuItemId, string menuItemName, decimal unitPrice, MenuItemType type)
    {
        MenuItemId = menuItemId;
        MenuItemName = menuItemName;
        UnitPrice = unitPrice;
        Type = type;
    }
}

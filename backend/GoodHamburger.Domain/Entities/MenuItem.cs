using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Domain.Entities;

public class MenuItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public MenuItemType Type { get; set; }

    public MenuItem() { }

    public MenuItem(int id, string name, decimal price, MenuItemType type)
    {
        Id = id;
        Name = name;
        Price = price;
        Type = type;
    }
}

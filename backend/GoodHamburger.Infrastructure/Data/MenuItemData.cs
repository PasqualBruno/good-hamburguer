using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Infrastructure.Data;

/// <summary>
/// Dados estáticos do cardápio. Simula um seed de banco de dados.
/// </summary>
public static class MenuItemData
{
    public static List<MenuItem> GetAll() =>
    [
        new MenuItem(1, "X Burger", 5.00m, MenuItemType.Sandwich),
        new MenuItem(2, "X Egg", 4.50m, MenuItemType.Sandwich),
        new MenuItem(3, "X Bacon", 7.00m, MenuItemType.Sandwich),
        new MenuItem(4, "Batata Frita", 2.00m, MenuItemType.Side),
        new MenuItem(5, "Refrigerante", 2.50m, MenuItemType.Drink),
    ];
}

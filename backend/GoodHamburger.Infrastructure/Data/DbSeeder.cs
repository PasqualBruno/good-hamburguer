using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using GoodHamburger.Infrastructure.Context;

namespace GoodHamburger.Infrastructure.Data;

public static class DbSeeder
{
    public static void Seed(GoodHamburgerDbContext context)
    {
        if (!context.MenuItems.Any())
        {
            context.MenuItems.AddRange(new List<MenuItem>
            {
                new MenuItem(1, "X Burger", 5.00m, MenuItemType.Sandwich),
                new MenuItem(2, "X Egg", 4.50m, MenuItemType.Sandwich),
                new MenuItem(3, "X Bacon", 7.00m, MenuItemType.Sandwich),
                new MenuItem(4, "Batata Frita", 2.00m, MenuItemType.Side),
                new MenuItem(5, "Refrigerante", 2.50m, MenuItemType.Drink)
            });
        }

        if (!context.Promotions.Any())
        {
            context.Promotions.AddRange(new List<Promotion>
            {
                new Promotion(1, "Combo Completo", 0.20m, new List<MenuItemType> { MenuItemType.Sandwich, MenuItemType.Side, MenuItemType.Drink }),
                new Promotion(2, "Combo Verão", 0.15m, new List<MenuItemType> { MenuItemType.Sandwich, MenuItemType.Drink }),
                new Promotion(3, "Combo Barriga Cheia", 0.10m, new List<MenuItemType> { MenuItemType.Sandwich, MenuItemType.Side })
            });
        }

        if (!context.Users.Any())
        {
            context.Users.Add(new User(Guid.NewGuid(), "admin@goodhamburger.com", "admin123", "Restaurante Admin"));
        }

        context.SaveChanges();
    }
}

using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Interfaces;
using GoodHamburger.Infrastructure.Data;

namespace GoodHamburger.Infrastructure.Repositories;

public class MenuItemRepository : IMenuItemRepository
{
    private readonly List<MenuItem> _items = MenuItemData.GetAll();

    public List<MenuItem> GetAll() => _items;

    public MenuItem? GetById(int id) => _items.FirstOrDefault(i => i.Id == id);
}

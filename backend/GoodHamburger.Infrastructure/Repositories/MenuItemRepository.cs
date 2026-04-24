using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Interfaces;
using GoodHamburger.Infrastructure.Context;

namespace GoodHamburger.Infrastructure.Repositories;

public class MenuItemRepository : IMenuItemRepository
{
    private readonly GoodHamburgerDbContext _context;

    public MenuItemRepository(GoodHamburgerDbContext context)
    {
        _context = context;
    }

    public List<MenuItem> GetAll()
    {
        return _context.MenuItems.ToList();
    }

    public MenuItem? GetById(int id)
    {
        return _context.MenuItems.Find(id);
    }
}

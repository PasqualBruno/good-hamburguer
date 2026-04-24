using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Domain.Interfaces;

namespace GoodHamburger.Application.Services;

public class MenuService : IMenuService
{
    private readonly IMenuItemRepository _menuItemRepository;

    public MenuService(IMenuItemRepository menuItemRepository)
    {
        _menuItemRepository = menuItemRepository;
    }

    public List<MenuItemDto> GetAllItems()
    {
        return _menuItemRepository.GetAll().Select(item => new MenuItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Price = item.Price,
            Type = item.Type.ToString()
        }).ToList();
    }
}

using GoodHamburger.Application.DTOs;

namespace GoodHamburger.Application.Interfaces;

public interface IMenuService
{
    List<MenuItemDto> GetAllItems();
}

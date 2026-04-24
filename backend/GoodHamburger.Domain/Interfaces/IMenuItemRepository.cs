using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Domain.Interfaces;

public interface IMenuItemRepository
{
    List<MenuItem> GetAll();
    MenuItem? GetById(int id);
}

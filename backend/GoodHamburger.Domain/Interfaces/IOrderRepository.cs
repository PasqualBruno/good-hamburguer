using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Domain.Interfaces;

public interface IOrderRepository
{
    void Add(Order order);
    void Update(Order order);
    Order? GetById(Guid id);
    List<Order> GetAll();
}

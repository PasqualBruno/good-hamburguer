using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Interfaces;

namespace GoodHamburger.Infrastructure.Repositories;

/// <summary>
/// Repositório de pedidos em memória. Será substituído por EF Core + SQL Server no futuro.
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly List<Order> _orders = new();

    public void Add(Order order)
    {
        _orders.Add(order);
    }

    public void Update(Order order)
    {
        var existingOrder = GetById(order.Id);
        if (existingOrder != null)
        {
            var index = _orders.IndexOf(existingOrder);
            _orders[index] = order;
        }
    }

    public Order? GetById(Guid id)
    {
        return _orders.FirstOrDefault(o => o.Id == id);
    }

    public List<Order> GetAll()
    {
        return _orders.ToList();
    }
}

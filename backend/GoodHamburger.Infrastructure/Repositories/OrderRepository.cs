using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Interfaces;
using GoodHamburger.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly GoodHamburgerDbContext _context;

    public OrderRepository(GoodHamburgerDbContext context)
    {
        _context = context;
    }

    public void Add(Order order)
    {
        _context.Orders.Add(order);
        _context.SaveChanges();
    }

    public void Update(Order order)
    {
        _context.Orders.Update(order);
        _context.SaveChanges();
    }

    public Order? GetById(Guid id)
    {
        return _context.Orders
            .Include(o => o.Items)
            .FirstOrDefault(o => o.Id == id);
    }

    public List<Order> GetAll()
    {
        return _context.Orders
            .Include(o => o.Items)
            .ToList();
    }
}

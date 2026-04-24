using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Interfaces;
using GoodHamburger.Infrastructure.Context;

namespace GoodHamburger.Infrastructure.Repositories;

public class PromotionRepository : IPromotionRepository
{
    private readonly GoodHamburgerDbContext _context;

    public PromotionRepository(GoodHamburgerDbContext context)
    {
        _context = context;
    }

    public List<Promotion> GetAll()
    {
        return _context.Promotions.ToList();
    }
}

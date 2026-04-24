using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Interfaces;
using GoodHamburger.Infrastructure.Data;

namespace GoodHamburger.Infrastructure.Repositories;

public class PromotionRepository : IPromotionRepository
{
    private readonly List<Promotion> _promotions = PromotionData.GetAll();

    public List<Promotion> GetAll() => _promotions;
}

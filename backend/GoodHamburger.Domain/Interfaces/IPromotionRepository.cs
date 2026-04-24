using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Domain.Interfaces;

public interface IPromotionRepository
{
    List<Promotion> GetAll();
}

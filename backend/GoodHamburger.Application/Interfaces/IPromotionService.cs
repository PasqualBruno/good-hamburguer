using GoodHamburger.Application.DTOs;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Application.Interfaces;

public interface IPromotionService
{
    List<PromotionDto> GetAllPromotions();
    Promotion? FindBestPromotion(IEnumerable<MenuItemType> itemTypes);
}

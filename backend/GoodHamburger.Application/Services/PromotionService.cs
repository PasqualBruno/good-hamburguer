using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using GoodHamburger.Domain.Interfaces;

namespace GoodHamburger.Application.Services;

public class PromotionService : IPromotionService
{
    private readonly IPromotionRepository _promotionRepository;
    private readonly IMenuItemRepository _menuItemRepository;

    public PromotionService(IPromotionRepository promotionRepository, IMenuItemRepository menuItemRepository)
    {
        _promotionRepository = promotionRepository;
        _menuItemRepository = menuItemRepository;
    }

    public List<PromotionDto> GetAllPromotions()
    {
        var allItems = _menuItemRepository.GetAll();
        var promotions = _promotionRepository.GetAll();

        return promotions.Select(promo => new PromotionDto
        {
            Id = promo.Id,
            Name = promo.Name,
            DiscountPercent = promo.DiscountPercent * 100, // 0.20 → 20
            RequiredItemTypes = promo.RequiredTypes.Select(t => t.ToString()).ToList(),
            Conditions = promo.RequiredTypes.Select(type => new ConditionDto
            {
                ItemType = type.ToString(),
                Description = GetTypeDescription(type),
                AvailableItems = allItems
                    .Where(item => item.Type == type)
                    .Select(item => new MenuItemDto
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Price = item.Price,
                        Type = item.Type.ToString()
                    }).ToList()
            }).ToList()
        }).ToList();
    }

    /// <summary>
    /// Encontra a melhor promoção aplicável (maior desconto) para os tipos de itens selecionados.
    /// </summary>
    public Promotion? FindBestPromotion(IEnumerable<MenuItemType> itemTypes)
    {
        var types = itemTypes.ToList();

        return _promotionRepository.GetAll()
            .Where(p => p.IsApplicable(types))
            .OrderByDescending(p => p.DiscountPercent)
            .FirstOrDefault();
    }

    private static string GetTypeDescription(MenuItemType type) => type switch
    {
        MenuItemType.Sandwich => "Qualquer sanduíche",
        MenuItemType.Side => "Qualquer acompanhamento",
        MenuItemType.Drink => "Qualquer bebida",
        _ => type.ToString()
    };
}

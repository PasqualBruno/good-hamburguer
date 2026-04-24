using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Infrastructure.Data;

/// <summary>
/// Dados estáticos das promoções. Simula um seed de banco de dados.
/// </summary>
public static class PromotionData
{
    public static List<Promotion> GetAll() =>
    [
        new Promotion(
            id: 1,
            name: "Combo Completo",
            discountPercent: 0.20m,
            requiredTypes: [MenuItemType.Sandwich, MenuItemType.Side, MenuItemType.Drink]
        ),
        new Promotion(
            id: 2,
            name: "Combo Verão",
            discountPercent: 0.15m,
            requiredTypes: [MenuItemType.Sandwich, MenuItemType.Drink]
        ),
        new Promotion(
            id: 3,
            name: "Combo Barriga Cheia",
            discountPercent: 0.10m,
            requiredTypes: [MenuItemType.Sandwich, MenuItemType.Side]
        ),
    ];
}

using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Domain.Entities;

public class Promotion
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal DiscountPercent { get; set; }
    public List<MenuItemType> RequiredTypes { get; set; } = new();

    public Promotion() { }

    public Promotion(int id, string name, decimal discountPercent, List<MenuItemType> requiredTypes)
    {
        Id = id;
        Name = name;
        DiscountPercent = discountPercent;
        RequiredTypes = requiredTypes;
    }

    /// <summary>
    /// Verifica se os tipos de itens fornecidos atendem aos requisitos desta promoção.
    /// </summary>
    public bool IsApplicable(IEnumerable<MenuItemType> itemTypes)
    {
        var typesSet = new HashSet<MenuItemType>(itemTypes);
        return RequiredTypes.All(required => typesSet.Contains(required));
    }
}

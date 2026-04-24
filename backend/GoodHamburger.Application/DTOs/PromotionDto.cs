namespace GoodHamburger.Application.DTOs;

public class PromotionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal DiscountPercent { get; set; }
    public List<string> RequiredItemTypes { get; set; } = new();
    public List<ConditionDto> Conditions { get; set; } = new();
}

public class ConditionDto
{
    public string ItemType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<MenuItemDto> AvailableItems { get; set; } = new();
}

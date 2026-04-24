namespace GoodHamburger.Application.DTOs;

public class CreateOrderRequest
{
    public List<int> MenuItemIds { get; set; } = new();
}

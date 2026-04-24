using GoodHamburger.Application.DTOs;

namespace GoodHamburger.Application.Interfaces;

public interface IOrderService
{
    OrderResponse CreateOrder(CreateOrderRequest request);
}

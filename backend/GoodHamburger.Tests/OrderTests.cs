using FluentAssertions;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using GoodHamburger.Domain.Errors;
using Xunit;

namespace GoodHamburger.Tests;

public class OrderTests
{
    [Fact]
    public void Create_ShouldCalculateTotalCorrectly_WithoutPromotion()
    {
        // Arrange
        var items = new List<OrderItem>
        {
            new OrderItem(1, "X Burger", 5.00m, MenuItemType.Sandwich),
            new OrderItem(4, "Batata", 2.00m, MenuItemType.Side)
        };

        // Act
        var order = Order.Create(items, null);

        // Assert
        order.Subtotal.Should().Be(7.00m);
        order.Total.Should().Be(7.00m);
        order.DiscountValue.Should().Be(0);
        order.Status.Should().Be(OrderStatus.Received);
    }

    [Fact]
    public void Create_ShouldApplyPromotion_WhenProvided()
    {
        // Arrange
        var items = new List<OrderItem>
        {
            new OrderItem(1, "X Burger", 5.00m, MenuItemType.Sandwich),
            new OrderItem(4, "Batata", 2.00m, MenuItemType.Side),
            new OrderItem(5, "Refri", 2.50m, MenuItemType.Drink)
        };
        var promo = new Promotion(1, "Combo Completo", 0.20m, new List<MenuItemType>());

        // Act
        var order = Order.Create(items, promo);

        // Assert
        order.Subtotal.Should().Be(9.50m);
        order.DiscountValue.Should().Be(1.90m); // 20% of 9.50
        order.Total.Should().Be(7.60m);
        order.PromotionName.Should().Be("Combo Completo");
    }

    [Fact]
    public void ChangeStatus_ShouldThrowError_WhenTransitionIsInvalid()
    {
        // Arrange
        var order = Order.Create(new List<OrderItem>(), null); // Status is Received

        // Act
        var action = () => order.ChangeStatus(OrderStatus.Delivered);

        // Assert
        action.Should().Throw<DomainError>()
            .And.Code.Should().Be(DomainErrorCodes.InvalidStatusTransition);
    }

    [Fact]
    public void ChangeStatus_ShouldUpdateStatus_WhenTransitionIsValid()
    {
        // Arrange
        var order = Order.Create(new List<OrderItem>(), null); // Received

        // Act
        order.ChangeStatus(OrderStatus.Preparing);

        // Assert
        order.Status.Should().Be(OrderStatus.Preparing);
    }

    [Fact]
    public void ChangeStatus_ShouldAllowCancellation_FromPreparing()
    {
        // Arrange
        var order = Order.Create(new List<OrderItem>(), null);
        order.ChangeStatus(OrderStatus.Preparing);

        // Act
        order.ChangeStatus(OrderStatus.Cancelled);

        // Assert
        order.Status.Should().Be(OrderStatus.Cancelled);
    }
}

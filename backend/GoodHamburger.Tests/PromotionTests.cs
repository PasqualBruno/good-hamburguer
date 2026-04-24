using FluentAssertions;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using Xunit;

namespace GoodHamburger.Tests;

public class PromotionTests
{
    [Fact]
    public void IsApplicable_ShouldReturnTrue_WhenAllRequiredTypesArePresent()
    {
        // Arrange
        var required = new List<MenuItemType> { MenuItemType.Sandwich, MenuItemType.Side };
        var promo = new Promotion(1, "Combo Barriga Cheia", 0.10m, required);
        var currentItems = new List<MenuItemType> { MenuItemType.Sandwich, MenuItemType.Side, MenuItemType.Drink };

        // Act
        var result = promo.IsApplicable(currentItems);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsApplicable_ShouldReturnFalse_WhenRequiredTypeIsMissing()
    {
        // Arrange
        var required = new List<MenuItemType> { MenuItemType.Sandwich, MenuItemType.Side, MenuItemType.Drink };
        var promo = new Promotion(1, "Combo Completo", 0.20m, required);
        var currentItems = new List<MenuItemType> { MenuItemType.Sandwich, MenuItemType.Drink };

        // Act
        var result = promo.IsApplicable(currentItems);

        // Assert
        result.Should().BeFalse();
    }
}

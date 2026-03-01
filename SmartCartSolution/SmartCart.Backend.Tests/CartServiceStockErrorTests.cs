using SmartCart.Backend.DTOs;
using SmartCart.Backend.Repositories;
using SmartCart.Backend.Services;
using Xunit;

namespace SmartCart.Backend.Tests;

public class CartServiceStockErrorTests
{
    private static ICartService CreateCartService()
    {
        var productRepo = new ProductRepository();
        var cartRepo = new CartRepository();
        var orderRepo = new OrderRepository();
        var couponRepo = new CouponRepository();
        return new CartService(cartRepo, productRepo, orderRepo, couponRepo);
    }

    [Fact]
    public void AddOrUpdateItem_QuantityExceedsStock_ReturnsConflict()
    {
        var service = CreateCartService();

        var result = service.AddOrUpdateItem(new AddCartItemRequest
        {
            ProductId = 1,
            Quantity = 9999
        });

        Assert.False(result.Success);
        Assert.True(result.IsConflict);
        Assert.NotNull(result.Error);
    }
}

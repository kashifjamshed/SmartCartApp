using SmartCart.Backend.DTOs;
using SmartCart.Backend.Repositories;
using SmartCart.Backend.Services;
using Xunit;

namespace SmartCart.Backend.Tests;

public class CartServiceCheckoutPricingTests
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
    public void Checkout_AppliesDiscountAndTax()
    {
        var service = CreateCartService();
        var addResult = service.AddOrUpdateItem(new AddCartItemRequest
        {
            ProductId = 1,
            Quantity = 2
        });
        var cartId = addResult.Data!.CartId;
        service.ApplyCoupon(cartId, new ApplyCouponRequest { Code = "FLAT50" });

        var checkoutResult = service.Checkout(cartId);

        Assert.True(checkoutResult.Success);
        Assert.NotNull(checkoutResult.Data);
        var order = checkoutResult.Data!;
        Assert.Equal(1000m, order.Subtotal);
        Assert.Equal(500m, order.Discount);
        Assert.True(order.Tax > 0);
        Assert.Equal(order.Subtotal - order.Discount + order.Tax, order.GrandTotal);
    }
}

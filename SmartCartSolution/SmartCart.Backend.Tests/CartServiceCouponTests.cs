using SmartCart.Backend.DTOs;
using SmartCart.Backend.Repositories;
using SmartCart.Backend.Services;
using Xunit;

namespace SmartCart.Backend.Tests;

public class CartServiceCouponTests
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
    public void ApplyCoupon_InvalidCode_ReturnsError()
    {
        var service = CreateCartService();
        var addResult = service.AddOrUpdateItem(new AddCartItemRequest
        {
            ProductId = 1,
            Quantity = 1
        });
        Assert.True(addResult.Success);
        var cartId = addResult.Data!.CartId;

        var couponResult = service.ApplyCoupon(cartId, new ApplyCouponRequest { Code = "INVALID" });

        Assert.False(couponResult.Success);
        Assert.NotNull(couponResult.Error);
    }

    [Fact]
    public void ApplyCoupon_ValidFlat50_Succeeds()
    {
        var service = CreateCartService();
        var addResult = service.AddOrUpdateItem(new AddCartItemRequest
        {
            ProductId = 1,
            Quantity = 1
        });
        var cartId = addResult.Data!.CartId;

        var couponResult = service.ApplyCoupon(cartId, new ApplyCouponRequest { Code = "FLAT50" });

        Assert.True(couponResult.Success);
        Assert.Null(couponResult.Error);
    }

    [Fact]
    public void ApplyCoupon_FLAT50_WhenSubtotalBelow500_ReturnsError()
    {
        var service = CreateCartService();
        var addResult = service.AddOrUpdateItem(new AddCartItemRequest
        {
            ProductId = 4,
            Quantity = 1
        });
        var cartId = addResult.Data!.CartId;

        var couponResult = service.ApplyCoupon(cartId, new ApplyCouponRequest { Code = "FLAT50" });

        Assert.False(couponResult.Success);
        Assert.NotNull(couponResult.Error);
        Assert.Contains("500", couponResult.Error);
    }

    [Fact]
    public void ApplyCoupon_ValidSave10_Succeeds()
    {
        var service = CreateCartService();
        var addResult = service.AddOrUpdateItem(new AddCartItemRequest
        {
            ProductId = 3,
            Quantity = 1
        });
        var cartId = addResult.Data!.CartId;

        var couponResult = service.ApplyCoupon(cartId, new ApplyCouponRequest { Code = "SAVE10" });

        Assert.True(couponResult.Success);
        Assert.Null(couponResult.Error);
    }

    [Fact]
    public void ApplyCoupon_SAVE10_WhenSubtotal1000OrLess_ReturnsError()
    {
        var service = CreateCartService();
        var addResult = service.AddOrUpdateItem(new AddCartItemRequest
        {
            ProductId = 1,
            Quantity = 2
        });
        var cartId = addResult.Data!.CartId;

        var couponResult = service.ApplyCoupon(cartId, new ApplyCouponRequest { Code = "SAVE10" });

        Assert.False(couponResult.Success);
        Assert.NotNull(couponResult.Error);
        Assert.Contains("1000", couponResult.Error);
    }
}

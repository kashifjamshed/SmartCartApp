using SmartCart.Backend.DTOs;

namespace SmartCart.Backend.Services;

public interface ICartService
{
    ServiceResult<AddCartItemResponse> AddOrUpdateItem(AddCartItemRequest request);
    ServiceResult UpdateItemQuantity(Guid cartId, int productId, UpdateCartItemRequest request);
    ServiceResult RemoveItem(Guid cartId, int productId);
    CartResponse? GetCart(Guid cartId);
    ServiceResult ApplyCoupon(Guid cartId, ApplyCouponRequest request);
    ServiceResult<OrderResponse> Checkout(Guid cartId);
}

using SmartCart.Backend.DTOs;

namespace SmartCart.Backend.Services;

public interface IOrderService
{
    OrderResponse? GetOrder(Guid orderId);
}

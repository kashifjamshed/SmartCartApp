using SmartCart.Backend.DTOs;
using SmartCart.Backend.Repositories;

namespace SmartCart.Backend.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public OrderResponse? GetOrder(Guid orderId)
    {
        var order = _orderRepository.GetById(orderId);
        if (order == null) return null;

        return new OrderResponse
        {
            OrderId = order.Id,
            PurchasedItems = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                LineTotal = i.LineTotal
            }).ToList(),
            Subtotal = order.Subtotal,
            Discount = order.Discount,
            Tax = order.Tax,
            GrandTotal = order.GrandTotal,
            AppliedCouponCode = order.AppliedCouponCode
        };
    }
}

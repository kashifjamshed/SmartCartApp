namespace SmartCart.Backend.DTOs;

public class OrderResponse
{
    public Guid OrderId { get; set; }
    public List<OrderItemDto> PurchasedItems { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal GrandTotal { get; set; }
    public string? AppliedCouponCode { get; set; }
}

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}

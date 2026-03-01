namespace SmartCart.Backend.Models;

public class Order
{
    public Guid Id { get; set; }
    public Guid CartId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal GrandTotal { get; set; }
    public string? AppliedCouponCode { get; set; }
}

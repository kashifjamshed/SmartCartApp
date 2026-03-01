namespace SmartCart.Backend.Models;

public class Cart
{
    public Guid Id { get; set; }
    public List<CartItem> Items { get; set; } = new();
    public string? AppliedCouponCode { get; set; }
}

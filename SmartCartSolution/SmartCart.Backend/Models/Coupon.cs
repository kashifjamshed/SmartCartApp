namespace SmartCart.Backend.Models;

public class Coupon
{
    public string Code { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Value { get; set; }

}

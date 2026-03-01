using System.ComponentModel.DataAnnotations;

namespace SmartCart.Backend.DTOs;

public class ApplyCouponRequest
{
    [Required]
    public string? Code { get; set; }
}

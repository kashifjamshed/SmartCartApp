using System.Collections.Concurrent;
using SmartCart.Backend.Models;

namespace SmartCart.Backend.Repositories;

public class CouponRepository : ICouponRepository
{
    private readonly ConcurrentDictionary<string, Coupon> _coupons = new(StringComparer.OrdinalIgnoreCase);

    public CouponRepository()
    {
        _coupons["FLAT50"] = new Coupon { Code = "FLAT50", Type = "Flat", Value = 50m };
        _coupons["SAVE10"] = new Coupon { Code = "SAVE10", Type = "Percent", Value = 10m };
    }

    public Coupon? GetByCode(string code) => _coupons.TryGetValue(code, out var c) ? c : null;
}

using SmartCart.Backend.Models;

namespace SmartCart.Backend.Repositories;

public interface ICouponRepository
{
    Coupon? GetByCode(string code);
}

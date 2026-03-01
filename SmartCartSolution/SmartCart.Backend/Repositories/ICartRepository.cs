using SmartCart.Backend.Models;

namespace SmartCart.Backend.Repositories;

public interface ICartRepository
{
    Cart? GetById(Guid id);
    Cart Create();
    void Update(Cart cart);
}

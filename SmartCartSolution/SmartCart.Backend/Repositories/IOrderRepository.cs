using SmartCart.Backend.Models;

namespace SmartCart.Backend.Repositories;

public interface IOrderRepository
{
    Order? GetById(Guid id);
    void Add(Order order);
}

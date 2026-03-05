using System.Collections.Concurrent;
using SmartCart.Backend.Models;

namespace SmartCart.Backend.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly Dictionary<Guid, Order> _orders = new();
    public Order? GetById(Guid id) => _orders.TryGetValue(id, out var order) ? order : null;

    public void Add(Order order)
    {
        _orders[order.Id] = order;
    }
}

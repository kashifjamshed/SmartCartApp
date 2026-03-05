using System.Collections.Concurrent;
using SmartCart.Backend.Models;

namespace SmartCart.Backend.Repositories;

public class CartRepository : ICartRepository
{
    private readonly Dictionary<Guid, Cart> _carts = new();

    public Cart? GetById(Guid id) => _carts.TryGetValue(id, out var cart) ? cart : null;

    public Cart Create()
    {
        var cart = new Cart { Id = Guid.NewGuid() };
        _carts[cart.Id] = cart;
        return cart;
    }

    public void Update(Cart cart)
    {
        _carts[cart.Id] = cart;
    }
}

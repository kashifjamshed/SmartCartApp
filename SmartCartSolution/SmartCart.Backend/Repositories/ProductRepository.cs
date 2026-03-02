using System.Collections.Concurrent;
using SmartCart.Backend.Models;

namespace SmartCart.Backend.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<int, Product> _products = new();

    public ProductRepository()
    {
        SeedProducts();
    }

    private void SeedProducts()
    {
        var items = new List<Product>
        {
            new() { Id = 1, Name = "Laptop", Price = 500m, Stock = 50 },
            new() { Id = 2, Name = "Gaming Desktop", Price = 600m, Stock = 30 },
            new() { Id = 3, Name = "PS5 Console", Price = 1200m, Stock = 25 },
            new() { Id = 4, Name = "Monitor", Price = 400m, Stock = 40 },
            new() { Id = 5, Name = "Television", Price = 800m, Stock = 20 },
            new() { Id = 6, Name = "Washing Machine", Price = 1200m, Stock = 15 },
            new() { Id = 7, Name = "Refrigerator", Price = 1500m, Stock = 10 },
            new() { Id = 8, Name = "Oven", Price = 800m, Stock = 20 }
        };

        foreach (var p in items)
            _products[p.Id] = p;
    }

    public IReadOnlyList<Product> GetAll() => _products.Values.OrderBy(p => p.Id).ToList();

    public Product? GetById(int id) => _products.TryGetValue(id, out var p) ? p : null;

    public void UpdateStock(int productId, int newStock)
    {
        if (_products.TryGetValue(productId, out var product))
        {
            product.Stock = newStock;
        }
    }
}

using SmartCart.Backend.Models;

namespace SmartCart.Backend.Repositories;

public interface IProductRepository
{
    IReadOnlyList<Product> GetAll();
    Product? GetById(int id);
    void UpdateStock(int productId, int newStock);
}

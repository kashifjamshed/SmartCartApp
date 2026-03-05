using SmartCart.Backend.DTOs;
using SmartCart.Backend.Repositories;

namespace SmartCart.Backend.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public IReadOnlyList<ProductDto> GetCatalog()
    {
        return _productRepository.GetAll()
            .Select(p => new ProductDto { Id = p.Id, Name = p.Name, Price = p.Price, Stock = p.Stock })
            .ToList();

    }
}

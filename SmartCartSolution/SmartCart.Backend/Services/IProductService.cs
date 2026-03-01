using SmartCart.Backend.DTOs;

namespace SmartCart.Backend.Services;

public interface IProductService
{
    IReadOnlyList<ProductDto> GetCatalog();
}

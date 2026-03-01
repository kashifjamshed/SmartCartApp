using Microsoft.AspNetCore.Mvc;
using SmartCart.Backend.DTOs;
using SmartCart.Backend.Services;

namespace SmartCart.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ProductDto>> Get()
    {
        return Ok(_productService.GetCatalog());
    }
}

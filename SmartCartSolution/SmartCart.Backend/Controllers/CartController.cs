using Microsoft.AspNetCore.Mvc;
using SmartCart.Backend.DTOs;
using SmartCart.Backend.Services;

namespace SmartCart.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }


    [HttpPost("items")]
    public IActionResult AddItem([FromBody] AddCartItemRequest request)
    {
        if (request == null)
            return BadRequest("Request body is required.");

        if (request.Quantity is null or <= 0)
            return BadRequest("Quantity must be greater than 0.");

        var result = _cartService.AddOrUpdateItem(request);

        if (result.IsConflict)
            return Conflict(result.Error ?? "Insufficient stock.");

        if (!result.Success)
            return BadRequest(result.Error);

        return Ok(result.Data);
    }


    [HttpPut("{cartId:guid}/items/{productId:int}")]
    public IActionResult UpdateItem(Guid cartId, int productId, [FromBody] UpdateCartItemRequest request)
    {
        if (request == null)
            return BadRequest("Request body is required.");

        var result = _cartService.UpdateItemQuantity(cartId, productId, request);

        if (result.IsConflict)
            return Conflict(result.Error ?? "Insufficient stock.");

        if (!result.Success)
            return BadRequest(result.Error ?? "Failed to update item.");

        return Ok();
    }

    [HttpDelete("{cartId:guid}/items/{productId:int}")]
    public IActionResult RemoveItem(Guid cartId, int productId)
    {
        var result = _cartService.RemoveItem(cartId, productId);
        if (!result.Success)
            return BadRequest(result.Error ?? "Failed to remove item.");
        return Ok();
    }

    [HttpGet("{cartId:guid}")]
    public IActionResult GetCart(Guid cartId)
    {
        var cart = _cartService.GetCart(cartId);
        if (cart == null)
            return NotFound();

        return Ok(cart);
    }


    [HttpPost("{cartId:guid}/apply-coupon")]
    public IActionResult ApplyCoupon(Guid cartId, [FromBody] ApplyCouponRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Code))
            return BadRequest("Coupon code is required.");

        var result = _cartService.ApplyCoupon(cartId, request);

        if (!result.Success)
            return BadRequest(result.Error ?? "Invalid coupon code.");

        return Ok();
    }


    [HttpPost("{cartId:guid}/checkout")]
    public IActionResult Checkout(Guid cartId)
    {
        var result = _cartService.Checkout(cartId);

        if (result.IsConflict)
            return Conflict(result.Error ?? "Insufficient stock.");

        if (!result.Success)
            return BadRequest(result.Error);

        return Ok(result.Data);
    }
}

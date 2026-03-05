using SmartCart.Backend.DTOs;
using SmartCart.Backend.Models;
using SmartCart.Backend.Repositories;

namespace SmartCart.Backend.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ICouponRepository _couponRepository;
    private static readonly object CheckoutLock = new();

    public CartService(
        ICartRepository cartRepository,
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        ICouponRepository couponRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _couponRepository = couponRepository;
    }

    public ServiceResult<AddCartItemResponse> AddOrUpdateItem(AddCartItemRequest request)
    {
        if (request.Quantity is null or <= 0)
            return ServiceResult<AddCartItemResponse>.FailureResult("Quantity must be greater than 0.");

        var product = _productRepository.GetById(request.ProductId!.Value);
        if (product == null)
            return ServiceResult<AddCartItemResponse>.FailureResult("Product not found.");

        if (product.Stock < request.Quantity.Value)
            return ServiceResult<AddCartItemResponse>.ConflictResult("Insufficient stock.");

        Cart cart;
        if (request.CartId.HasValue && request.CartId.Value != Guid.Empty)
        {
            cart = _cartRepository.GetById(request.CartId.Value) ?? _cartRepository.Create();
        }

        else
        {
            cart = _cartRepository.Create();
        }

        var existing = cart.Items.FirstOrDefault(x => x.ProductId == request.ProductId.Value);
        var newQty = (existing?.Quantity ?? 0) + request.Quantity.Value;

        if (product.Stock < newQty)
            return ServiceResult<AddCartItemResponse>.ConflictResult("Insufficient stock.");

        if (existing != null)
            existing.Quantity = newQty;
        else
            cart.Items.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = request.Quantity.Value
            });

        _cartRepository.Update(cart);
        return ServiceResult<AddCartItemResponse>.SuccessResult(new AddCartItemResponse { CartId = cart.Id });
    }

    public ServiceResult UpdateItemQuantity(Guid cartId, int productId, UpdateCartItemRequest request)
    {
        if (request.Quantity is null)
            return ServiceResult.FailureResult("Quantity is required.");

        var cart = _cartRepository.GetById(cartId);
        if (cart == null)
            return ServiceResult.FailureResult("Cart not found.");

        var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);
        if (item == null)
            return ServiceResult.FailureResult("Item not in cart.");

        if (request.Quantity.Value == 0)
        {
            cart.Items.Remove(item);
            _cartRepository.Update(cart);
            return ServiceResult.SuccessResult();
        }

        var product = _productRepository.GetById(productId);
        if (product == null)
            return ServiceResult.FailureResult("Product not found.");
        if (product.Stock < request.Quantity.Value)
            return ServiceResult.ConflictResult("Insufficient stock.");

        item.Quantity = request.Quantity.Value;
        _cartRepository.Update(cart);
        return ServiceResult.SuccessResult();
    }

    public ServiceResult RemoveItem(Guid cartId, int productId)
    {
        var cart = _cartRepository.GetById(cartId);
        if (cart == null)
            return ServiceResult.FailureResult("Cart not found.");

        var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);
        if (item == null)
            return ServiceResult.FailureResult("Item not in cart.");

        cart.Items.Remove(item);
        _cartRepository.Update(cart);
        return ServiceResult.SuccessResult();
    }

    public CartResponse? GetCart(Guid cartId)
    {
        var cart = _cartRepository.GetById(cartId);
        if (cart == null) return null;

        var subtotal = cart.Items.Sum(i => i.LineTotal);
        var discount = CalculateDiscount(subtotal, cart.AppliedCouponCode);
        var grandTotal = Math.Max(0, subtotal - discount);

        return new CartResponse
        {
            CartId = cart.Id,
            Items = cart.Items.Select(i => new CartItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                LineTotal = i.LineTotal
            }).ToList(),
            Subtotal = subtotal,
            Discount = discount,
            GrandTotal = grandTotal,
            AppliedCouponCode = cart.AppliedCouponCode
        };
    }

    public ServiceResult ApplyCoupon(Guid cartId, ApplyCouponRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
            return ServiceResult.FailureResult("Coupon code is required.");

        var coupon = _couponRepository.GetByCode(request.Code.Trim());
        if (coupon == null)
            return ServiceResult.FailureResult("Invalid coupon code.");

        if (coupon.Code != "FLAT50" && coupon.Code != "SAVE10")
            return ServiceResult.FailureResult("Invalid coupon code.");

        var cart = _cartRepository.GetById(cartId);
        if (cart == null)
            return ServiceResult.FailureResult("Cart not found.");

        var subtotal = cart.Items.Sum(i => i.LineTotal);
        if (coupon.Code == "FLAT50" && subtotal < 500)
            return ServiceResult.FailureResult("FLAT50 requires a cart subtotal of at least 500 INR.");
        if (coupon.Code == "SAVE10" && subtotal <= 1000)
            return ServiceResult.FailureResult("SAVE10 requires a cart subtotal greater than 1000 INR.");

        cart.AppliedCouponCode = coupon.Code;
        _cartRepository.Update(cart);
        return ServiceResult.SuccessResult();
    }


    public ServiceResult<OrderResponse> Checkout(Guid cartId)
    {
        lock (CheckoutLock)
        {
            var cart = _cartRepository.GetById(cartId);
            if (cart == null)
                return ServiceResult<OrderResponse>.FailureResult("Cart not found.");

            if (cart.Items.Count == 0)
                return ServiceResult<OrderResponse>.FailureResult("Cart is empty.");

            foreach (var item in cart.Items)
            {
                var product = _productRepository.GetById(item.ProductId);
                if (product == null || product.Stock < item.Quantity)
                    return ServiceResult<OrderResponse>.ConflictResult("Insufficient stock for one or more items.");
            }

            foreach (var item in cart.Items)
            {
                var product = _productRepository.GetById(item.ProductId)!;
                _productRepository.UpdateStock(item.ProductId, product.Stock - item.Quantity);
            }

            var subtotal = cart.Items.Sum(i => i.LineTotal);
            var discount = CalculateDiscount(subtotal, cart.AppliedCouponCode);
            var afterDiscount = Math.Max(0, subtotal - discount);
            const decimal TaxRate = 0.08m;
            var tax = Math.Round(afterDiscount * TaxRate, 2);
            var grandTotal = afterDiscount + tax;

            var order = new Order
            {
                Id = Guid.NewGuid(),
                CartId = cartId,
                Items = cart.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    LineTotal = i.UnitPrice * i.Quantity
                }).ToList(),
                Subtotal = subtotal,
                Discount = discount,
                Tax = tax,
                GrandTotal = grandTotal,
                AppliedCouponCode = cart.AppliedCouponCode
            };

            _orderRepository.Add(order);

            var orderResponse = new OrderResponse
            {
                OrderId = order.Id,
                PurchasedItems = order.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    LineTotal = i.LineTotal
                }).ToList(),
                Subtotal = order.Subtotal,
                Discount = order.Discount,
                Tax = order.Tax,
                GrandTotal = order.GrandTotal,
                AppliedCouponCode = order.AppliedCouponCode
            };

            return ServiceResult<OrderResponse>.SuccessResult(orderResponse);
        }
    }

    private static decimal CalculateDiscount(decimal subtotal, string? couponCode)
    {
        if (string.IsNullOrEmpty(couponCode)) return 0;

        if (couponCode == "FLAT50")
            return subtotal >= 500 ? Math.Round(subtotal * 0.5m, 2) : 0;

        if (couponCode == "SAVE10")
            return subtotal > 1000 ? Math.Min(200, Math.Round(subtotal * 0.10m, 2)) : 0;

        return 0;
    }
}

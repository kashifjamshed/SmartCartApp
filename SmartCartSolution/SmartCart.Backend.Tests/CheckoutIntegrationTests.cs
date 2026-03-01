using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using SmartCart.Backend.DTOs;
using Xunit;

namespace SmartCart.Backend.Tests;

public class CheckoutIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CheckoutIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Checkout_WhenStockExceeded_Returns409()
    {
        var addRequest = new AddCartItemRequest
        {
            ProductId = 1,
            Quantity = 9999
        };

        var response = await _client.PostAsJsonAsync("/api/cart/items", addRequest);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task FullCheckoutFlow_SucceedsAndReturnsOrder()
    {
        var addRequest = new AddCartItemRequest
        {
            ProductId = 1,
            Quantity = 2
        };
        var addResponse = await _client.PostAsJsonAsync("/api/cart/items", addRequest);
        addResponse.EnsureSuccessStatusCode();
        var addResult = await addResponse.Content.ReadFromJsonAsync<AddCartItemResponse>();
        var cartId = addResult!.CartId;

        var checkoutResponse = await _client.PostAsync($"/api/cart/{cartId}/checkout", null);
        checkoutResponse.EnsureSuccessStatusCode();
        var order = await checkoutResponse.Content.ReadFromJsonAsync<OrderResponse>();

        Assert.NotNull(order);
        Assert.NotEqual(Guid.Empty, order.OrderId);
        Assert.NotEmpty(order.PurchasedItems);
        Assert.True(order.GrandTotal > 0);
    }
}

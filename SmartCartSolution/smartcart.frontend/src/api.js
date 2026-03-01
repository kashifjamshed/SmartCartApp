import { API_BASE_URL } from "./config";

// Type definitions were removed when converting from TypeScript to plain JavaScript.

async function request(path, options) {
  const url = `${API_BASE_URL}${path}`;
  const res = await fetch(url, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      ...options?.headers,
    },
  });
  const text = await res.text();
  let data;
  if (text) {
    try {
      data = JSON.parse(text);
    } catch {
      data = undefined;
    }
  }
const errMsg = !res.ok
  ? ((data && data.message) || text) || res.statusText
  : undefined;

return { data, status: res.status, error: errMsg };
}

export async function getProducts() {
  const { data, status, error } = await request("/products");
  if (status !== 200 || !data) throw new Error(error ?? "Failed to load products");
  return data;
}

export async function addOrUpdateCartItem(productId, quantity, cartId) {
  const body = JSON.stringify({
    productId,
    quantity,
    cartId: cartId || null,
  });
  const { data, status, error } = await request("/cart/items", {
    method: "POST",
    body,
  });
  if (status === 409) throw new Error(error ?? "Insufficient stock");
  if (status !== 200 || !data) throw new Error(error ?? "Failed to add item");
  return data;
}

export async function updateCartItemQuantity(cartId, productId, quantity) {
  const { status, error } = await request(
    `/cart/${cartId}/items/${productId}`,
    {
      method: "PUT",
      body: JSON.stringify({ quantity }),
    }
  );
  if (status === 409) throw new Error(error ?? "Insufficient stock");
  if (status !== 200) throw new Error(error ?? "Failed to update item");
}

export async function removeCartItem(cartId, productId) {
  const { status, error } = await request(
    `/cart/${cartId}/items/${productId}`,
    { method: "DELETE" }
  );
  if (status !== 200) throw new Error(error ?? "Failed to remove item");
}

export async function getCart(cartId) {
  const { data, status, error } = await request(`/cart/${cartId}`);
  if (status === 404 || !data) throw new Error(error ?? "Cart not found");
  return data;
}

export async function applyCoupon(cartId, code) {
  const { status, error } = await request(`/cart/${cartId}/apply-coupon`, {
    method: "POST",
    body: JSON.stringify({ code }),
  });
  if (status !== 200) throw new Error(error ?? "Invalid coupon");
}

export async function checkout(cartId) {
  const { data, status, error } = await request(`/cart/${cartId}/checkout`, {
    method: "POST",
  });
  if (status === 409) throw new Error(error ?? "Insufficient stock");
  if (status !== 200 || !data) throw new Error(error ?? "Checkout failed");
  return data;
}

export async function getOrder(orderId) {
  const { data, status, error } = await request(`/orders/${orderId}`);
  if (status === 404 || !data) throw new Error(error ?? "Order not found");
  return data;
}

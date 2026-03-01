import React, { useCallback, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  addOrUpdateCartItem,
  applyCoupon,
  checkout,
  getCart,
  updateCartItemQuantity,
  removeCartItem,
} from "../api";
import { useSelector, useDispatch } from "react-redux";
import { setCartId } from "../store/cartSlice";
import "./Cart.css";

export default function Cart() {
  const cartId = useSelector((s) => s.cart.cartId);
  const dispatch = useDispatch();
  const [cart, setCart] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [couponCode, setCouponCode] = useState("");
  const [couponError, setCouponError] = useState(null);
  const [checkingOut, setCheckingOut] = useState(false);
  const [addingProductId, setAddingProductId] = useState(null);
  const [deletingProductId, setDeletingProductId] = useState(null);
  const [updatingProductId, setUpdatingProductId] = useState(null);
  const navigate = useNavigate();

  const loadCart = useCallback(async () => {
    if (!cartId) {
      setLoading(false);
      setCart(null);
      return;
    }
    setError(null);
    try {
      const data = await getCart(cartId);
      setCart(data);
    } catch (err) {
      setCart(null);
    } finally {
      setLoading(false);
    }
  }, [cartId]);

  useEffect(() => {
    if (cartId) loadCart();
    else {
      setLoading(false);
      setCart(null);
    }
  }, [cartId, loadCart]);

  const handleApplyCoupon = useCallback(async () => {
    if (!cartId || !couponCode.trim()) return;
    setCouponError(null);
    try {
      await applyCoupon(cartId, couponCode.trim());
      await loadCart();
    } catch (err) {
      setCouponError(err instanceof Error ? err.message : "Invalid coupon");
    }
  }, [cartId, couponCode, loadCart]);

  const handleAddMore = useCallback(
    async (productId) => {
      if (!cartId) return;
      setAddingProductId(productId);
      setError(null);
      try {
        await addOrUpdateCartItem(productId, 1, cartId);
        await loadCart();
      } catch (err) {
        setError(err instanceof Error ? err.message : "Failed to update");
      } finally {
        setAddingProductId(null);
      }
    },
    [cartId, loadCart]
  );

  const handleDecrease = useCallback(
    async (productId, currentQty) => {
      if (!cartId || currentQty <= 1) return;
      setUpdatingProductId(productId);
      setError(null);
      try {
        await updateCartItemQuantity(cartId, productId, currentQty - 1);
        await loadCart();
      } catch (err) {
        setError(err instanceof Error ? err.message : "Failed to update");
      } finally {
        setUpdatingProductId(null);
      }
    },
    [cartId, loadCart]
  );

  const handleRemove = useCallback(
    async (productId) => {
      if (!cartId) return;
      setDeletingProductId(productId);
      setError(null);
      try {
        await removeCartItem(cartId, productId);
        await loadCart();
      } catch (err) {
        setError(err instanceof Error ? err.message : "Failed to remove item");
      } finally {
        setDeletingProductId(null);
      }
    },
    [cartId, loadCart]
  );

 const handleCheckout = useCallback(async () => {
  if (!cartId || !cart || cart.items.length === 0) return;
  setCheckingOut(true);
  setError(null);
  try {
    const order = await checkout(cartId);
    setCart(null);
    dispatch(setCartId(null));
    navigate(`/checkout/${order.orderId}`, { state: { order } });
  } catch (err) {
    setError(err instanceof Error ? err.message : "Checkout failed");
  } finally {
    setCheckingOut(false);
  }
}, [cartId, cart, navigate, setCartId]);

  if (loading) return <div className="page page-loading">Loading…</div>;
  if (!cartId || !cart)
    return (
      <div className="page cart-page">
        <p className="cart-empty-message">No cart. Add items from the product list.</p>
        <button type="button" className="btn-back" onClick={() => navigate("/")}>
          View Products
        </button>
      </div>
    );

  return (
    <div className="page cart-page">
      <h1>Cart</h1>
      {error && <div className="cart-error">{error}</div>}
      {cart.items.length === 0 ? (
        <p className="cart-empty-message">Your cart is empty.</p>
      ) : (
        <>
          <ul className="cart-items">
            {cart.items.map((item) => (
              <li key={item.productId} className="cart-item">
                <span className="item-name">{item.productName}</span>
                <span className="item-price">₹{item.unitPrice.toFixed(2)}</span>
                <div className="item-qty-controls">
                  <button
                    type="button"
                    className="btn-qty btn-qty-minus"
                    disabled={item.quantity <= 1 || updatingProductId === item.productId}
                    onClick={() => handleDecrease(item.productId, item.quantity)}
                    title="Decrease quantity"
                  >
                    −
                  </button>
                  <span className="item-qty-value">{item.quantity}</span>
                  <button
                    type="button"
                    className="btn-qty btn-qty-plus"
                    disabled={addingProductId === item.productId}
                    onClick={() => handleAddMore(item.productId)}
                    title="Increase quantity"
                  >
                    +
                  </button>
                </div>
                <span className="item-total">₹{item.lineTotal.toFixed(2)}</span>
                <button
                  type="button"
                  className="btn-delete"
                  disabled={deletingProductId === item.productId}
                  onClick={() => handleRemove(item.productId)}
                  title="Remove from cart"
                >
                  {deletingProductId === item.productId ? "…" : "Delete"}
                </button>
              </li>
            ))}
          </ul>
          <div className="cart-summary">
            <div className="summary-row">
              <span>Subtotal</span>
              <span>₹{cart.subtotal.toFixed(2)}</span>
            </div>
            <div className="coupon-row">
              <input
                type="text"
                placeholder="Coupon code"
                value={couponCode}
                onChange={(e) => {
                  setCouponCode(e.target.value);
                  setCouponError(null);
                }}
              />
              <button type="button" onClick={handleApplyCoupon}>
                Apply
              </button>
            </div>
            {couponError ? (
              <div className="coupon-error">{couponError}</div>
            ) : cart.appliedCouponCode ? (
              <div className="applied-coupon">Applied: {cart.appliedCouponCode}</div>
            ) : null}
            <div className="summary-row">
              <span>Discount</span>
              <span>-₹{cart.discount.toFixed(2)}</span>
            </div>
            <div className="summary-row total">
              <span>Grand Total</span>
              <span>₹{cart.grandTotal.toFixed(2)}</span>
            </div>
          </div>
          <div className="cart-actions">
            <button
              type="button"
              className="btn-checkout"
              disabled={checkingOut}
              onClick={handleCheckout}
            >
              {checkingOut ? "Processing…" : "Checkout"}
            </button>
            <button type="button" className="btn-back" onClick={() => navigate("/")}>
              Back to Products
            </button>
          </div>
        </>
      )}
      {cart && cart.items.length === 0 && (
        <button type="button" className="btn-back" onClick={() => navigate("/")}>
          Back to Products
        </button>
      )}
    </div>
  );
}

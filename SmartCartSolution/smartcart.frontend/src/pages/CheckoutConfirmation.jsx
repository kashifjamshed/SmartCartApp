import React, { useEffect, useState } from "react";
import { useLocation, useNavigate, useParams } from "react-router-dom";
import { getOrder } from "../api";
import "./CheckoutConfirmation.css";

export default function CheckoutConfirmation() {
  const { orderId } = useParams();
  const location = useLocation();
  const navigate = useNavigate();
  const [order, setOrder] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const stateOrder = location.state && location.state.order;
    if (stateOrder) {
      setOrder(stateOrder);
      setLoading(false);
      return;
    }
    if (!orderId) {
      setLoading(false);
      setError("No order ID");
      return;
    }
    getOrder(orderId)
      .then(setOrder)
      .catch((err) => setError(err instanceof Error ? err.message : "Failed to load order"))
      .finally(() => setLoading(false));
  }, [orderId, location.state]);

  if (loading) return <div className="page page-loading">Loading…</div>;
  if (error || !order)
    return (
      <div className="page checkout-page">
        <p className="error">{error ?? "Order not found"}</p>
        <button type="button" className="btn-back" onClick={() => navigate("/")}>
          Back to Products
        </button>
      </div>
    );

  return (
    <div className="page checkout-page">
      <h1>Order Confirmation</h1>
      <p className="order-id">Order ID: {order.orderId}</p>
      <ul className="order-items">
        {order.purchasedItems.map((item) => (
          <li key={`${item.productId}-${item.quantity}`} className="order-item">
            <span className="item-name">{item.productName}</span>
            <span className="item-qty">× {item.quantity}</span>
            <span className="item-total">₹{item.lineTotal.toFixed(2)}</span>
          </li>
        ))}
      </ul>
      <div className="order-summary">
        <div className="summary-row">
          <span>Subtotal</span>
          <span>₹{order.subtotal.toFixed(2)}</span>
        </div>
        <div className="summary-row">
          <span>Discount</span>
          <span>-₹{order.discount.toFixed(2)}</span>
        </div>
        <div className="summary-row">
          <span>Tax</span>
          <span>₹{order.tax.toFixed(2)}</span>
        </div>
        <div className="summary-row total">
          <span>Grand Total</span>
          <span>₹{order.grandTotal.toFixed(2)}</span>
        </div>
      </div>
      <button type="button" className="btn-back" onClick={() => navigate("/")}>
        Back to Products
      </button>
    </div>
  );
}

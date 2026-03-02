import React, { useCallback, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { addOrUpdateCartItem, getProducts } from "../api";
import { useSelector, useDispatch } from "react-redux";
import { setCartId } from "../store/cartSlice";
import "./ProductList.css";

export default function ProductList() {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [addingId, setAddingId] = useState(null);
  const navigate = useNavigate();
  const cartId = useSelector((s) => s.cart.cartId);
  const dispatch = useDispatch();

  useEffect(() => {
    let cancelled = false;
    getProducts()
      .then((data) => {
        if (!cancelled) setProducts(data);
      })
      .catch((err) => {
        if (!cancelled) setError(err instanceof Error ? err.message : "Failed to load");
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });
    return () => {
      cancelled = true;
    };
  }, []);

  const handleAddToCart = useCallback(
    async (productId) => {
      setAddingId(productId);
      setError(null);
      try {
        const res = await addOrUpdateCartItem(productId, 1, cartId);
        dispatch(setCartId(res.cartId));
        navigate("/cart");
      } catch (err) {
        setError(err instanceof Error ? err.message : "Failed to add");
      } finally {
        setAddingId(null);
      }
    },
    [cartId, dispatch, navigate]
  );

  if (loading) return <div className="page page-loading">Loading…</div>;
  if (error) return <div className="page"><div className="error">Error: {error}</div></div>;

  return (
    <div className="page product-list-page">
      <div className="product-grid">
        {products.map((p) => (
          <div key={p.id} className="product-card">
            <img
              src={`/images/${encodeURIComponent(
                p.name
                  .toLowerCase() 
              )}.jpg`}
              alt={p.name}
              className="product-image"
            />
            <div className="product-info">
              <span className="product-name">{p.name}</span>
              <span className="product-price">₹{p.price.toFixed(2)}</span>
              <span className="product-stock">Stock: {p.stock}</span>
            </div>
            <button
              type="button"
              className="btn-add"
              disabled={p.stock < 1 || addingId === p.id}
              onClick={() => handleAddToCart(p.id)}
            >
              {addingId === p.id ? "Adding…" : "Add to Cart"}
            </button>
          </div>
        ))}
      </div>
    </div>
  );
}

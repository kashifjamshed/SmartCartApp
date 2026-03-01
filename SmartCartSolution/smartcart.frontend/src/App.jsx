import React from "react";
import { BrowserRouter, Routes, Route, useNavigate } from "react-router-dom";
import { Provider } from "react-redux";
import store from "./store";
import Cart from "./pages/Cart";
import CheckoutConfirmation from "./pages/CheckoutConfirmation";
import ProductList from "./pages/ProductList";
import "./App.css";

function AppHeader() {
  const navigate = useNavigate();
  return (
    <header className="app-header">
      <span className="app-header-title">SmartCart</span>
      <button type="button" className="app-header-cart" onClick={() => navigate("/cart")}>
        Cart
      </button>
    </header>
  );
}

function App() {
  return (
    <BrowserRouter>
      <Provider store={store}>
        <div className="App">
          <AppHeader />
          <main className="app-main">
            <Routes>
              <Route path="/" element={<ProductList />} />
              <Route path="/cart" element={<Cart />} />
              <Route path="/checkout/:orderId" element={<CheckoutConfirmation />} />
            </Routes>
          </main>
        </div>
      </Provider>
    </BrowserRouter>
  );
}

export default App;

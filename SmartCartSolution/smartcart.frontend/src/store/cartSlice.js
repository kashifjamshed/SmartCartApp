const initialState = {
  cartId: (() => {
    try {
      return sessionStorage.getItem("smartcart_cart_id");
    } catch {
      return null;
    }
  })(),
};

function cartReducer(state = initialState, action) {
  switch (action.type) {
    case "cart/setCartId":
      try {
        if (action.payload) sessionStorage.setItem("smartcart_cart_id", action.payload);
        else sessionStorage.removeItem("smartcart_cart_id");
      } catch {
        // ignore
      }
      return { ...state, cartId: action.payload };
    default:
      return state;
  }
}

export const setCartId = (id) => ({ type: "cart/setCartId", payload: id });

export default cartReducer;

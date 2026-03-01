Prerequisites
- Install .NET 8 SDK and check with: dotnet --version
- Install Node.js version 18 or higher and npm, check with: node --version and npm --version


Installation & Setup
Step 1: Download or Clone the Project
Use Git → git clone <repository-url>, then cd SmartCartSolution


Step 2: Backend Setup
- cd SmartCart.Backend
- Run dotnet restore
- Run dotnet build


Step 3: Frontend Setup
- cd smartcart.frontend
- Run npm install
- npm start


Running the Application
Option A: Run Both Backend and Frontend
- Terminal 1: cd SmartCart.Backend → dotnet run → backend starts on https://localhost:7079
- Terminal 2: cd smartcart-frontend → npm start → frontend starts on http://localhost:3000



Running Tests
- cd SmartCart.Backend.Tests
- Run dotnet test to execute all tests


Testing Scenarios
- Pricing & Calculations: discounts, tax, grand total
- Coupon Validation: valid/invalid codes, removal, totals update
- Stock Management: insufficient stock, decrements,increment
- Integration Tests done

API Endpoints
- Products: GET /api/products
- Cart: POST /api/cart/items, PUT /api/cart/{cartId}/items/{productId}, DELETE /api/cart/{cartId}/items/{productId}, GET /api/cart/{cartId}
- Coupon: POST /api/cart/{cartId}/apply-coupon, DELETE /api/cart/{cartId}/coupon
- Checkout: POST /api/cart/{cartId}/checkout
- Orders: GET /api/orders/{orderId}



Environment Configuration
- Backend: runs on https://localhost:7079, CORS origin http://localhost:3000
- Frontend: API base URL https://localhost:7079/api


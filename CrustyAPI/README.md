# 🍔 Crusty API

The **Crusty API** is the backend REST API for the Crusty Crab restaurant application.

It provides the backend functionality required by the web and Android applications, including customer accounts, authentication, menu management, shopping carts, orders, payments, staff/admin management, delivery addresses, and the Crabby Bot dialogue assistant.

The API is built using **ASP.NET Core Web API** and connects to a **MySQL** database using **Entity Framework Core**.

---

## 🛠️ Technologies Used

* ASP.NET Core Web API
* C#
* .NET 10
* Entity Framework Core 9
* MySQL
* Pomelo.EntityFrameworkCore.MySql
* BCrypt.Net-Next
* REST API architecture

---

# 📁 Project Structure

```text
CrustyAPI/
│
├── Controllers/
│   ├── AuthController.cs
│   ├── CustomerController.cs
│   ├── DeliveryAddressController.cs
│   ├── MenuController.cs
│   ├── CartController.cs
│   ├── OrderController.cs
│   ├── PaymentController.cs
│   ├── AdminStaffController.cs
│   └── CrabbyBotController.cs
│
├── Data/
│   └── AppDbContext.cs
│
├── DTOs/
│   ├── AuthDTOs.cs
│   ├── CustomerDTOs.cs
│   ├── DeliveryAddressDTOs.cs
│   ├── MenuDTOs.cs
│   ├── CartDTOs.cs
│   ├── OrderDTOs.cs
│   ├── PaymentDTOs.cs
│   ├── AdminStaffDTOs.cs
│   └── CrabbyBotDTOs.cs
│
├── Models/
│   ├── Customer.cs
│   ├── DeliveryAddress.cs
│   ├── MenuItem.cs
│   ├── Cart.cs
│   ├── CartItem.cs
│   ├── Order.cs
│   ├── OrderItem.cs
│   ├── Payment.cs
│   ├── PaymentMethod.cs
│   ├── Staff.cs
│   └── AdminUser.cs
│
├── Services/
│   ├── AuthService.cs
│   ├── CustomerService.cs
│   ├── DeliveryAddressService.cs
│   ├── MenuService.cs
│   ├── CartService.cs
│   ├── OrderService.cs
│   ├── PaymentService.cs
│   ├── AdminStaffService.cs
│   └── CrabbyBotService.cs
│
└── Program.cs
```

---

# 🚀 Running the API

Make sure MySQL is running and the `crusty_crab_db` database has been created.

The API runs on:

```text
HTTPS: https://localhost:7098
HTTP:  http://localhost:5083
```

For Postman testing, the HTTPS address can be used:

```text
https://localhost:7098
```

---

# 🔐 Authentication

## Customer Login

### `POST /api/auth/customer/login`

Logs a customer into the application.

The user can provide their email address as the identifier together with their password.

### Request

```json
{
  "identifier": "patrick@krustykrab.com",
  "password": "patrick123"
}
```

### Response

```json
{
  "success": true,
  "userId": 1,
  "username": "Patrick",
  "role": "Customer",
  "message": "Login successful."
}
```

---

## Admin Login

### `POST /api/auth/admin/login`

Authenticates an administrator using their admin account.

### Request

```json
{
  "identifier": "admin",
  "password": "password123"
}
```

The API verifies the password against the securely hashed password stored in the database.

---

# 👤 Customer Endpoints

## Get All Customers

### `GET /api/customer`

Returns all registered customers.

Passwords are not returned to the client.

---

## Get Customer

### `GET /api/customer/{id}`

Returns a specific customer using their Customer ID.

Example:

```text
GET /api/customer/1
```

---

## Create Customer

### `POST /api/customer`

Creates a new customer account.

### Request

```json
{
  "firstName": "Patrick",
  "lastName": "Star",
  "phoneNumber": "0812345678",
  "emailAddress": "patrick@example.com",
  "password": "patrick123"
}
```

The API hashes the password before storing it in the database.

Duplicate email addresses are rejected.

---

## Update Customer

### `PUT /api/customer/{id}`

Updates an existing customer's information.

Example:

```text
PUT /api/customer/1
```

---

## Delete Customer

### `DELETE /api/customer/{id}`

Deletes a customer.

The database is configured so that related delivery addresses and carts can be removed through the customer relationship.

---

# 📍 Delivery Address Endpoints

## Get All Delivery Addresses

### `GET /api/deliveryaddress`

Returns all delivery addresses stored in the database.

---

## Get Delivery Address

### `GET /api/deliveryaddress/{id}`

Returns a specific delivery address.

Example:

```text
GET /api/deliveryaddress/1
```

---

## Get Customer Addresses

### `GET /api/deliveryaddress/customer/{customerId}`

Returns all delivery addresses belonging to a particular customer.

Example:

```text
GET /api/deliveryaddress/customer/1
```

---

## Create Delivery Address

### `POST /api/deliveryaddress`

Creates a new delivery address for a customer.

### Request

```json
{
  "suburb": "Sandton",
  "houseNumber": "123",
  "streetName": "Main Street",
  "buildingType": "House",
  "customerID": 1
}
```

---

## Update Delivery Address

### `PUT /api/deliveryaddress/{id}`

Updates an existing delivery address.

---

## Delete Delivery Address

### `DELETE /api/deliveryaddress/{id}`

Deletes a delivery address.

---

# 🍔 Menu Endpoints

## Get Menu

### `GET /api/menu`

Returns the available menu items.

This endpoint is used by the web and Android applications to retrieve menu data.

Menu items contain information such as:

* Item name
* Description
* Price
* Category
* Availability
* Image URL

---

## Get Menu Item

### `GET /api/menu/{id}`

Returns one specific menu item.

Example:

```text
GET /api/menu/1
```

---

## Create Menu Item

### `POST /api/menu`

Creates a new menu item.

### Request

```json
{
  "itemName": "Krabby Patty",
  "description": "A delicious Krabby Patty.",
  "price": 59.99,
  "category": "Burgers",
  "availability": true,
  "imageUrl": "/Images/Menu/krabby-patty.jpg"
}
```

---

## Update Menu Item

### `PUT /api/menu/{id}`

Updates an existing menu item.

This can be used by administrators to change:

* Name
* Description
* Price
* Category
* Availability
* Image URL

---

## Delete Menu Item

### `DELETE /api/menu/{id}`

Deletes a menu item.

---

# 🛒 Cart Endpoints

## Create Cart

### `POST /api/cart`

Creates a shopping cart for a customer.

The API prevents a customer from unnecessarily creating multiple carts.

---

## Get Cart

### `GET /api/cart/{id}`

Returns a cart and its cart items.

Example:

```text
GET /api/cart/1
```

---

## Get Customer Cart

### `GET /api/cart/customer/{customerId}`

Returns the cart belonging to a particular customer.

Example:

```text
GET /api/cart/customer/1
```

---

## Add Item to Cart

### `POST /api/cart/{cartId}/items`

Adds a menu item to a customer's cart.

### Request

```json
{
  "menuItemID": 1,
  "quantity": 2
}
```

The API obtains the current menu price from the database rather than trusting a price supplied by the client.

---

## Update Cart Item

### `PUT /api/cart/items/{cartItemId}`

Changes the quantity of an item already in the cart.

### Request

```json
{
  "quantity": 3
}
```

The cart total is recalculated after the update.

---

## Remove Cart Item

### `DELETE /api/cart/items/{cartItemId}`

Removes an item from the cart.

The cart total is recalculated afterwards.

---

# 📦 Order Endpoints

## Get All Orders

### `GET /api/order`

Returns all orders.

This can be used by the administration side of the application to view orders.

---

## Get Order

### `GET /api/order/{id}`

Returns a specific order and its order items.

Example:

```text
GET /api/order/1
```

---

## Get Customer Orders

### `GET /api/order/customer/{customerId}`

Returns the orders belonging to a particular customer.

Example:

```text
GET /api/order/customer/1
```

---

## Create Order

### `POST /api/order`

Creates a new order.

The API calculates the prices of the order items using the current prices stored in the menu database.

This prevents the client from submitting an incorrect item price.

### Example Request

```json
{
  "customerID": 1,
  "deliveryMethod": "Delivery",
  "deliveryAddressID": 1,
  "items": [
    {
      "menuItemID": 1,
      "quantity": 2,
      "notes": "Extra sauce"
    }
  ]
}
```

The API calculates the order total automatically.

---

## Update Order Status

### `PUT /api/order/{id}/status`

Updates the status of an order.

### Request

```json
{
  "orderStatus": "Preparing"
}
```

Example order statuses can include:

```text
Pending
Preparing
Ready
Completed
Cancelled
```

---

# 💳 Payment Endpoints

## Get Payment Methods

### `GET /api/payment/methods`

Returns all available payment methods.

---

## Get Payment Method

### `GET /api/payment/methods/{id}`

Returns a specific payment method.

---

## Create Payment Method

### `POST /api/payment/methods`

Adds a new payment method.

### Request

```json
{
  "methodName": "Card"
}
```

---

## Delete Payment Method

### `DELETE /api/payment/methods/{id}`

Deletes a payment method.

---

## Create Payment

### `POST /api/payment`

Creates a payment associated with an order.

### Request

```json
{
  "amount": 119.98,
  "paymentStatus": "Pending",
  "paymentMethodID": 1,
  "orderID": 1
}
```

---

## Get Payment

### `GET /api/payment/{id}`

Returns a specific payment.

Example:

```text
GET /api/payment/1
```

---

## Get Order Payments

### `GET /api/payment/order/{orderId}`

Returns payments associated with a specific order.

Example:

```text
GET /api/payment/order/1
```

---

## Update Payment Status

### `PUT /api/payment/{id}/status`

Updates the status of a payment.

### Request

```json
{
  "paymentStatus": "Paid"
}
```

---

# 👨‍🍳 Staff & Admin Endpoints

Staff members and administrator accounts are separate entities.

A **Staff** record contains employee information, while an **Admin User** is an account that can be used to log into administrative functionality.

Not every staff member needs an administrator account.

---

## Staff

### `GET /api/adminstaff/staff`

Returns all staff members.

### `GET /api/adminstaff/staff/{id}`

Returns a specific staff member.

### `POST /api/adminstaff/staff`

Creates a staff member.

Example:

```json
{
  "firstName": "Squidward",
  "lastName": "Tentacles",
  "role": "Manager",
  "phoneNumber": "0812345678"
}
```

### `PUT /api/adminstaff/staff/{id}`

Updates a staff member.

### `DELETE /api/adminstaff/staff/{id}`

Deletes a staff member.

---

# 🔑 Admin User

## Get Admin Users

### `GET /api/adminstaff/admins`

Returns all administrator accounts.

Passwords are never returned.

---

## Get Admin User

### `GET /api/adminstaff/admins/{id}`

Returns a specific administrator account.

---

## Create Admin User

### `POST /api/adminstaff/admins`

Creates an administrator account.

The password is hashed before being stored.

An administrator can optionally be linked to an existing staff member.

---

## Update Admin User

### `PUT /api/adminstaff/admins/{id}`

Updates an administrator account.

---

## Delete Admin User

### `DELETE /api/adminstaff/admins/{id}`

Deletes an administrator account.

Deleting an admin account does not necessarily delete the associated staff member.

---

# 🦞 Crabby Bot

Crabby Bot is a predefined **dialogue/FAQ system** designed to help customers with common restaurant questions.

It does not use generative AI. Instead, the API provides predefined responses and options based on the option selected by the customer.

This makes the responses predictable and allows the restaurant to control the information provided to customers.

---

## Crabby Bot Response

### `POST /api/CrabbyBot`

Returns a response based on the selected option.

### Request

```json
{
  "optionId": "start"
}
```

### Main Menu Response

The `start` option provides choices such as:

* 🍔 Menu
* 🚚 Delivery
* 🛒 Ordering
* 💳 Payments
* 🕐 Opening Hours
* 📞 Co

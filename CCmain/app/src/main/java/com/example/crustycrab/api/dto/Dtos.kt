package com.example.crustycrab.api.dto

import com.google.gson.annotations.SerializedName

// ============================
// AUTH
// ============================

data class LoginRequest(
    val identifier: String,
    val password: String
)

data class LoginResponse(
    val success: Boolean,
    val userId: Int,
    val username: String,
    val role: String,
    val message: String
)

// ============================
// CUSTOMER
// ============================

data class CustomerDto(
    @SerializedName("customerID") val customerId: Int,
    val firstName: String,
    val lastName: String,
    val phoneNumber: String,
    val emailAddress: String,
    val createdAt: String? = null
)

data class CreateCustomerRequest(
    val firstName: String,
    val lastName: String,
    val phoneNumber: String,
    val emailAddress: String,
    val password: String
)

data class UpdateCustomerRequest(
    val firstName: String,
    val lastName: String,
    val phoneNumber: String,
    val emailAddress: String
)

// ============================
// MENU
// ============================

data class MenuItemDto(
    @SerializedName("menuItemID") val menuItemId: Int,
    val itemName: String,
    val description: String?,
    val price: Double,
    val category: String,
    val availability: Boolean,
    val imageUrl: String? = null
)

// ============================
// CART
// ============================

data class CreateCartRequest(
    @SerializedName("customerID") val customerId: Int
)

data class AddCartItemRequest(
    @SerializedName("menuItemID") val menuItemId: Int,
    val quantity: Int
)

data class UpdateCartItemRequest(
    val quantity: Int
)

data class CartItemDto(
    @SerializedName("cartItemID") val cartItemId: Int,
    @SerializedName("menuItemID") val menuItemId: Int,
    val itemName: String,
    val itemPrice: Double,
    val quantity: Int,
    val subtotal: Double
)

data class CartDto(
    @SerializedName("cartID") val cartId: Int,
    @SerializedName("customerID") val customerId: Int,
    val createdDate: String?,
    val totalAmount: Double,
    val items: List<CartItemDto> = emptyList()
)

// ============================
// ORDERS
// ============================

data class CreateOrderItemRequest(
    @SerializedName("menuItemID") val menuItemId: Int,
    val quantity: Int,
    val notes: String? = null
)

data class CreateOrderRequest(
    @SerializedName("customerID") val customerId: Int,
    @SerializedName("deliveryAddressID") val deliveryAddressId: Int? = null,
    val deliveryMethod: String,
    val items: List<CreateOrderItemRequest>
)

data class OrderItemDto(
    val orderItemID: Int,
    val menuItemID: Int,
    val itemName: String,
    val quantity: Int,
    val itemPrice: Double,
    val subtotal: Double,
    val notes: String? = null
)

data class OrderDto(
    val orderID: Int,
    val orderDate: String?,
    val orderStatus: String,
    val deliveryMethod: String,
    val totalAmount: Double,
    val deliveryAddressID: Int?,
    val customerID: Int,
    val staffId: Int? = null,
    val items: List<OrderItemDto> = emptyList()
)

data class UpdateOrderStatusRequest(
    val orderStatus: String
)

// ============================
// DELIVERY ADDRESS
// ============================

data class DeliveryAddressDto(
    @SerializedName("deliveryAddressID") val deliveryAddressId: Int,
    val suburb: String,
    val houseNumber: String,
    val streetName: String,
    val buildingType: String? = null,
    @SerializedName("customerID") val customerId: Int
)

data class CreateDeliveryAddressRequest(
    val suburb: String,
    val houseNumber: String,
    val streetName: String,
    val buildingType: String? = null,
    @SerializedName("customerID") val customerId: Int
)

data class UpdateDeliveryAddressRequest(
    val suburb: String,
    val houseNumber: String,
    val streetName: String,
    val buildingType: String? = null
)

// ============================
// PAYMENT
// ============================

data class PaymentMethodDto(
    val paymentMethodID: Int,
    val methodName: String
)

data class PaymentDto(
    val paymentID: Int,
    val paymentDate: String?,
    val amount: Double,
    val paymentStatus: String,
    val paymentMethodID: Int,
    val methodName: String,
    val orderID: Int
)

// ============================
// CRABBY BOT
// ============================

data class CrabbyBotRequest(
    val optionId: String
)

data class CrabbyBotOptionDto(
    val id: String,
    val text: String
)

data class CrabbyBotResponse(
    val message: String,
    val options: List<CrabbyBotOptionDto> = emptyList()
)

data class CreateSavedCardRequest(
    val customerID: Int,
    val paymentMethodID: Int,
    val cardholderName: String,
    val cardNumber: String,
    val expiryMonth: Int,
    val expiryYear: Int
)

data class CustomerPaymentMethodResponse(
    val customerPaymentMethodID: Int,
    val customerID: Int,
    val paymentMethodID: Int,
    val methodName: String,
    val cardholderName: String?,
    val cardLast4: String?,
    val expiryMonth: Int?,
    val expiryYear: Int?
)
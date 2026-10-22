package com.example.crustycrab.repository

import com.example.crustycrab.api.ApiClient
import com.example.crustycrab.api.dto.*
import com.example.crustycrab.util.ApiResult
import com.example.crustycrab.util.safeApiCall
import com.example.crustycrab.util.safeApiCallUnit

/**
 * The single point of contact between the UI and the CrustyAPI backend.
 * Activities never talk to Retrofit directly ΓÇö they call this.
 */
object CrustyRepository {

    private val api = ApiClient.service

    // ============================
    // AUTH
    // ============================

    suspend fun loginCustomer(email: String, password: String): ApiResult<LoginResponse> =
        safeApiCall { api.customerLogin(LoginRequest(email, password)) }

    // ============================
    // CUSTOMER
    // ============================

    suspend fun createCustomer(req: CreateCustomerRequest): ApiResult<CustomerDto> =
        safeApiCall { api.createCustomer(req) }

    suspend fun getCustomer(id: Int): ApiResult<CustomerDto> =
        safeApiCall { api.getCustomer(id) }

    suspend fun updateCustomer(id: Int, req: UpdateCustomerRequest): ApiResult<CustomerDto> =
        safeApiCall { api.updateCustomer(id, req) }

    // ============================
    // MENU
    // ============================

    suspend fun getMenu(): ApiResult<List<MenuItemDto>> =
        safeApiCall { api.getMenu() }

    suspend fun getMenuItem(id: Int): ApiResult<MenuItemDto> =
        safeApiCall { api.getMenuItem(id) }

    // ============================
    // CART
    // ============================

    suspend fun getCart(customerId: Int): ApiResult<CartDto> =
        safeApiCall { api.getCartByCustomer(customerId) }

    suspend fun createCart(customerId: Int): ApiResult<CartDto> =
        safeApiCall { api.createCart(CreateCartRequest(customerId)) }

    suspend fun addToCart(cartId: Int, menuItemId: Int, quantity: Int): ApiResult<CartDto> =
        safeApiCall { api.addItemToCart(cartId, AddCartItemRequest(menuItemId, quantity)) }

    suspend fun updateCartItem(cartItemId: Int, quantity: Int): ApiResult<CartDto> =
        safeApiCall { api.updateCartItem(cartItemId, UpdateCartItemRequest(quantity)) }

    suspend fun removeCartItem(cartItemId: Int): ApiResult<CartDto> =
        safeApiCall { api.removeCartItem(cartItemId) }

    // ============================
    // ORDERS
    // ============================

    suspend fun getOrders(customerId: Int): ApiResult<List<OrderDto>> =
        safeApiCall { api.getOrdersByCustomer(customerId) }

    suspend fun getOrder(id: Int): ApiResult<OrderDto> =
        safeApiCall { api.getOrder(id) }

    suspend fun createOrder(req: CreateOrderRequest): ApiResult<OrderDto> =
        safeApiCall { api.createOrder(req) }

    suspend fun updateOrderStatus(id: Int, status: String): ApiResult<OrderDto> =
        safeApiCall { api.updateOrderStatus(id, UpdateOrderStatusRequest(status)) }

    // ============================
    // DELIVERY ADDRESS
    // ============================

    suspend fun getAddresses(customerId: Int): ApiResult<List<DeliveryAddressDto>> =
        safeApiCall { api.getAddressesByCustomer(customerId) }

    suspend fun createAddress(req: CreateDeliveryAddressRequest): ApiResult<DeliveryAddressDto> =
        safeApiCall { api.createAddress(req) }

    suspend fun updateAddress(
        id: Int,
        req: UpdateDeliveryAddressRequest
    ): ApiResult<DeliveryAddressDto> =
        safeApiCall { api.updateAddress(id, req) }

    suspend fun deleteAddress(id: Int): ApiResult<Unit> =
        safeApiCallUnit { api.deleteAddress(id) }

    // ============================
    // PAYMENTS
    // ============================

    suspend fun getPaymentMethods(): ApiResult<List<PaymentMethodDto>> =
        safeApiCall { api.getPaymentMethods() }

    suspend fun getPayments(orderId: Int): ApiResult<List<PaymentDto>> =
        safeApiCall { api.getPaymentsByOrder(orderId) }

    // ============================
    // CRABBY BOT
    // ============================

    suspend fun crabbyBot(optionId: String): ApiResult<CrabbyBotResponse> =
        safeApiCall { api.crabbyBot(CrabbyBotRequest(optionId)) }

    suspend fun getCustomerPaymentMethods(
        customerId: Int
    ): ApiResult<List<CustomerPaymentMethodResponse>> {
        return safeApiCall {
            api.getCustomerPaymentMethods(customerId)
        }
    }

    suspend fun saveCustomerPaymentMethod(
        request: CreateSavedCardRequest
    ): ApiResult<CustomerPaymentMethodResponse> {
        return safeApiCall {
            api.saveCustomerPaymentMethod(request)
        }
    }

    suspend fun deleteCustomerPaymentMethod(
        id: Int,
        customerId: Int
    ): ApiResult<Unit> {
        return safeApiCall {
            api.deleteCustomerPaymentMethod(id, customerId)
        }


    }
    suspend fun cancelOrder(
        orderId: Int,
        customerId: Int
    ): ApiResult<OrderDto> {

        return try {

            val response =
                api.cancelOrder(orderId, customerId)

            if (response.isSuccessful && response.body() != null) {

                ApiResult.Success(response.body()!!)

            } else {

                ApiResult.Error(
                    response.errorBody()?.string()
                        ?: "Unable to cancel order."
                )
            }

        } catch (e: Exception) {

            ApiResult.Error(
                e.message ?: "Unable to cancel order."
            )
        }
    }
}

package com.example.crustycrab.api

import com.example.crustycrab.api.dto.*
import retrofit2.Response
import retrofit2.http.*


interface CrustyApiService {

    // ---------- AUTH ----------
    @POST("api/Auth/customer/login")
    suspend fun customerLogin(@Body req: LoginRequest): Response<LoginResponse>

    @POST("api/Auth/admin/login")
    suspend fun adminLogin(@Body req: LoginRequest): Response<LoginResponse>

    // ---------- CUSTOMER ----------
    @GET("api/Customer")
    suspend fun getCustomers(): Response<List<CustomerDto>>

    @GET("api/Customer/{id}")
    suspend fun getCustomer(@Path("id") id: Int): Response<CustomerDto>

    @POST("api/Customer")
    suspend fun createCustomer(@Body req: CreateCustomerRequest): Response<CustomerDto>

    @PUT("api/Customer/{id}")
    suspend fun updateCustomer(
        @Path("id") id: Int,
        @Body req: UpdateCustomerRequest
    ): Response<CustomerDto>

    @DELETE("api/Customer/{id}")
    suspend fun deleteCustomer(@Path("id") id: Int): Response<Unit>

    // ---------- MENU ----------
    @GET("api/Menu")
    suspend fun getMenu(): Response<List<MenuItemDto>>

    @GET("api/Menu/{id}")
    suspend fun getMenuItem(@Path("id") id: Int): Response<MenuItemDto>

    // ---------- CART ----------
    @GET("api/Cart/customer/{customerId}")
    suspend fun getCartByCustomer(@Path("customerId") customerId: Int): Response<CartDto>

    @GET("api/Cart/{id}")
    suspend fun getCart(@Path("id") id: Int): Response<CartDto>

    @POST("api/Cart")
    suspend fun createCart(@Body req: CreateCartRequest): Response<CartDto>

    @POST("api/Cart/{cartId}/items")
    suspend fun addItemToCart(
        @Path("cartId") cartId: Int,
        @Body req: AddCartItemRequest
    ): Response<CartDto>

    @PUT("api/Cart/items/{cartItemId}")
    suspend fun updateCartItem(
        @Path("cartItemId") cartItemId: Int,
        @Body req: UpdateCartItemRequest
    ): Response<CartDto>

    @DELETE("api/Cart/items/{cartItemId}")
    suspend fun removeCartItem(@Path("cartItemId") cartItemId: Int): Response<CartDto>

    // ---------- ORDERS ----------
    @GET("api/Order/customer/{customerId}")
    suspend fun getOrdersByCustomer(@Path("customerId") customerId: Int): Response<List<OrderDto>>

    @GET("api/Order/{id}")
    suspend fun getOrder(@Path("id") id: Int): Response<OrderDto>

    @POST("api/Order")
    suspend fun createOrder(@Body req: CreateOrderRequest): Response<OrderDto>

    @PUT("api/Order/{id}/status")
    suspend fun updateOrderStatus(
        @Path("id") id: Int,
        @Body req: UpdateOrderStatusRequest
    ): Response<OrderDto>

    // ---------- DELIVERY ADDRESS ----------
    @GET("api/DeliveryAddress/customer/{customerId}")
    suspend fun getAddressesByCustomer(
        @Path("customerId") customerId: Int
    ): Response<List<DeliveryAddressDto>>

    @POST("api/DeliveryAddress")
    suspend fun createAddress(
        @Body req: CreateDeliveryAddressRequest
    ): Response<DeliveryAddressDto>

    @PUT("api/DeliveryAddress/{id}")
    suspend fun updateAddress(
        @Path("id") id: Int,
        @Body req: UpdateDeliveryAddressRequest
    ): Response<DeliveryAddressDto>

    @DELETE("api/DeliveryAddress/{id}")
    suspend fun deleteAddress(@Path("id") id: Int): Response<Unit>

    // ---------- PAYMENTS ----------
    @GET("api/Payment/methods")
    suspend fun getPaymentMethods(): Response<List<PaymentMethodDto>>

    @GET("api/Payment/order/{orderId}")
    suspend fun getPaymentsByOrder(@Path("orderId") orderId: Int): Response<List<PaymentDto>>

    // ---------- CRABBY BOT ----------
    @POST("api/CrabbyBot")
    suspend fun crabbyBot(@Body req: CrabbyBotRequest): Response<CrabbyBotResponse>

    @GET("api/Payment/customer/{customerId}")
    suspend fun getCustomerPaymentMethods(
        @Path("customerId") customerId: Int
    ): Response<List<CustomerPaymentMethodResponse>>

    @POST("api/Payment/customer")
    suspend fun saveCustomerPaymentMethod(
        @Body request: CreateSavedCardRequest
    ): Response<CustomerPaymentMethodResponse>

    @DELETE("api/Payment/customer/{id}")
    suspend fun deleteCustomerPaymentMethod(
        @Path("id") id: Int,
        @Query("customerId") customerId: Int
    ): Response<Unit>

    @PUT("api/Order/{id}/cancel")
    suspend fun cancelOrder(
        @Path("id") orderId: Int,
        @Query("customerId") customerId: Int
    ): Response<OrderDto>
}

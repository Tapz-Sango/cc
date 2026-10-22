package com.example.crustycrab

import com.example.crustycrab.api.dto.CartDto
import com.example.crustycrab.api.dto.CartItemDto
import com.example.crustycrab.util.OrderRequestBuilder
import org.junit.Assert.assertEquals
import org.junit.Test

class OrderRequestBuilderTest {

    @Test
    fun build_createsCorrectOrderRequest() {

        val cart = CartDto(
            cartId = 1,
            customerId = 10,
            createdDate = null,
            totalAmount = 180.0,
            items = listOf(
                CartItemDto(
                    cartItemId = 1,
                    menuItemId = 5,
                    itemName = "Crab Burger",
                    itemPrice = 90.0,
                    quantity = 2,
                    subtotal = 180.0
                )
            )
        )

        val request = OrderRequestBuilder.build(
            cart = cart,
            customerId = 10,
            deliveryAddressId = 3
        )

        assertEquals(10, request.customerId)
        assertEquals(3, request.deliveryAddressId)
        assertEquals("Delivery", request.deliveryMethod)
        assertEquals(1, request.items.size)
        assertEquals(5, request.items[0].menuItemId)
        assertEquals(2, request.items[0].quantity)
    }
    @Test
    fun build_withEmptyCart_createsRequestWithNoItems() {

        val cart = CartDto(
            cartId = 1,
            customerId = 10,
            createdDate = null,
            totalAmount = 0.0,
            items = emptyList()
        )

        val request = OrderRequestBuilder.build(
            cart = cart,
            customerId = 10,
            deliveryAddressId = 3
        )

        assertEquals(10, request.customerId)
        assertEquals(3, request.deliveryAddressId)
        assertEquals("Delivery", request.deliveryMethod)
        assertEquals(0, request.items.size)
    }
}
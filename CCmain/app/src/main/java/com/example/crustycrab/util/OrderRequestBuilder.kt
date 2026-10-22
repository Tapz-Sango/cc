

package com.example.crustycrab.util

import com.example.crustycrab.api.dto.CartDto
import com.example.crustycrab.api.dto.CreateOrderItemRequest
import com.example.crustycrab.api.dto.CreateOrderRequest

object OrderRequestBuilder {

    fun build(
        cart: CartDto,
        customerId: Int,
        deliveryAddressId: Int
    ): CreateOrderRequest {

        return CreateOrderRequest(
            customerId = customerId,
            deliveryAddressId = deliveryAddressId,
            deliveryMethod = "Delivery",
            items = cart.items.map {
                CreateOrderItemRequest(
                    menuItemId = it.menuItemId,
                    quantity = it.quantity
                )
            }
        )
    }
}
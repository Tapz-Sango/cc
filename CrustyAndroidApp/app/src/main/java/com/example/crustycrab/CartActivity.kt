package com.example.crustycrab

import android.content.Intent
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.lifecycleScope
import com.example.crustycrab.api.dto.CartDto
import com.example.crustycrab.api.dto.CreateOrderItemRequest
import com.example.crustycrab.api.dto.CreateOrderRequest
import com.example.crustycrab.databinding.ActivityCartBinding
import com.example.crustycrab.databinding.ItemCartLineBinding
import com.example.crustycrab.repository.CrustyRepository
import com.example.crustycrab.session.SessionManager
import com.example.crustycrab.util.ApiResult
import kotlinx.coroutines.launch
import com.example.crustycrab.util.OrderRequestBuilder
import com.example.crustycrab.api.dto.DeliveryAddressDto
import android.app.AlertDialog
class CartActivity : AppCompatActivity() {

    private lateinit var binding: ActivityCartBinding
    private var cart: CartDto? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityCartBinding.inflate(layoutInflater)
        setContentView(binding.root)

        binding.btnBack.setOnClickListener { finish() }
        binding.btnCheckout.setOnClickListener { placeOrder() }

        loadCart()
    }

    override fun onResume() {
        super.onResume()
        loadCart()
    }

    private fun loadCart() {
        val uid = SessionManager.getUserId(this)
        if (uid <= 0) {
            toast("Please log in first.")
            finish()
            return
        }

        lifecycleScope.launch {
            when (val r = CrustyRepository.getCart(uid)) {
                is ApiResult.Success -> {
                    cart = r.data
                    renderCart()
                }
                is ApiResult.Error -> {
                    // No cart exists yet, try to create one
                    val created = CrustyRepository.createCart(uid)
                    if (created is ApiResult.Success) {
                        cart = created.data
                        renderCart()
                    } else {
                        binding.tvEmpty.visibility = View.VISIBLE
                        binding.llCartContainer.removeAllViews()
                    }
                }
            }
        }
    }

    private fun renderCart() {
        val c = cart ?: return
        binding.llCartContainer.removeAllViews()

        if (c.items.isEmpty()) {
            binding.tvEmpty.visibility = View.VISIBLE
            binding.tvTotal.text = getString(R.string.cart_total, 0)
            binding.btnCheckout.isEnabled = false
            return
        }

        binding.tvEmpty.visibility = View.GONE
        binding.btnCheckout.isEnabled = true

        val inflater = LayoutInflater.from(this)
        c.items.forEach { item ->
            val row = ItemCartLineBinding.inflate(inflater, binding.llCartContainer, false)
            row.tvName.text = item.itemName
            row.tvPrice.text = getString(R.string.price_format, item.subtotal.toInt())
            row.tvQty.text = item.quantity.toString()
            row.btnPlus.setOnClickListener { changeQty(item.cartItemId, item.quantity + 1) }
            row.btnMinus.setOnClickListener {
                if (item.quantity > 1) changeQty(item.cartItemId, item.quantity - 1)
                else removeItem(item.cartItemId)
            }
            row.btnRemove.setOnClickListener { removeItem(item.cartItemId) }
            binding.llCartContainer.addView(row.root)
        }

        binding.tvTotal.text = getString(R.string.cart_total, c.totalAmount.toInt())
    }

    private fun changeQty(cartItemId: Int, qty: Int) {
        lifecycleScope.launch {
            when (val r = CrustyRepository.updateCartItem(cartItemId, qty)) {
                is ApiResult.Success -> {
                    cart = r.data
                    renderCart()
                }
                is ApiResult.Error -> toast(r.message)
            }
        }
    }

    private fun removeItem(cartItemId: Int) {
        lifecycleScope.launch {
            when (val r = CrustyRepository.removeCartItem(cartItemId)) {
                is ApiResult.Success -> {
                    cart = r.data
                    renderCart()
                }
                is ApiResult.Error -> toast(r.message)
            }
        }
    }

    private fun placeOrder() {

        val c = cart ?: return
        val uid = SessionManager.getUserId(this)

        if (c.items.isEmpty()) {
            toast(getString(R.string.cart_empty_toast))
            return
        }

        lifecycleScope.launch {

            when (val result = CrustyRepository.getAddresses(uid)) {

                is ApiResult.Success -> {

                    if (result.data.isEmpty()) {
                        toast("Please add a delivery address first.")
                        return@launch
                    }

                    showAddressSelection(result.data)
                }

                is ApiResult.Error -> {
                    toast(result.message)
                }
            }
        }
    }
    private fun showAddressSelection(
        addresses: List<DeliveryAddressDto>
    ) {

        val addressLabels = addresses.map { addr ->
            "${addr.houseNumber} ${addr.streetName}, ${addr.suburb}"
        }.toTypedArray()

        var selectedAddressId = addresses[0].deliveryAddressId

        AlertDialog.Builder(this)
            .setTitle("Select Delivery Address")
            .setSingleChoiceItems(
                addressLabels,
                0
            ) { dialog, which ->
                selectedAddressId =
                    addresses[which].deliveryAddressId

                dialog.dismiss()

                createOrder(selectedAddressId)
            }
            .setNegativeButton("Cancel", null)
            .show()
    }

    private fun createOrder(deliveryAddressId: Int) {

        val c = cart ?: return
        val uid = SessionManager.getUserId(this)

        binding.btnCheckout.isEnabled = false
        binding.btnCheckout.text =
            getString(R.string.cart_placing_order)

        lifecycleScope.launch {

            val req = OrderRequestBuilder.build(
                cart = c,
                customerId = uid,
                deliveryAddressId = deliveryAddressId
            )

            when (val r = CrustyRepository.createOrder(req)) {

                is ApiResult.Success -> {

                    for (item in c.items) {
                        CrustyRepository.removeCartItem(item.cartItemId)
                    }

                    toast(
                        getString(
                            R.string.cart_order_placed,
                            r.data.orderID
                        )
                    )

                    startActivity(
                        Intent(
                            this@CartActivity,
                            TrackerActivity::class.java
                        )
                    )

                    finish()
                }

                is ApiResult.Error -> {

                    toast(r.message)

                    binding.btnCheckout.isEnabled = true
                    binding.btnCheckout.text =
                        getString(R.string.cart_place_order)
                }
            }
        }
    }
    private fun toast(m: String) =
        Toast.makeText(this, m, Toast.LENGTH_SHORT).show()
}

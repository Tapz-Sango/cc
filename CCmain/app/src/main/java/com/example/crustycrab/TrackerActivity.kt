
package com.example.crustycrab

import android.content.Intent
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.widget.TextView
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.lifecycleScope
import com.example.crustycrab.databinding.ActivityTrackerBinding
import com.example.crustycrab.databinding.ItemOrderBinding
import com.example.crustycrab.repository.CrustyRepository
import com.example.crustycrab.session.SessionManager
import com.example.crustycrab.util.ApiResult
import kotlinx.coroutines.launch

class TrackerActivity : AppCompatActivity() {
    private lateinit var binding: ActivityTrackerBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        binding = ActivityTrackerBinding.inflate(layoutInflater)
        setContentView(binding.root)

        binding.btnBack.setOnClickListener {
            finish()
        }

        binding.navHome.setOnClickListener {
            startActivity(Intent(this, MainActivity::class.java))
        }

        binding.navMenu.setOnClickListener {
            startActivity(Intent(this, MenuActivity::class.java))
        }

        binding.navCart.setOnClickListener {
            startActivity(Intent(this, CartActivity::class.java))
        }

        binding.navTracker.setOnClickListener {
            // Already on Tracker
        }

        binding.navAccount.setOnClickListener {
            startActivity(Intent(this, AccountActivity::class.java))
        }

        loadOrders()
    }



    private fun loadOrders() {

        val userId = SessionManager.getUserId(this)

        if (userId <= 0) {
            showEmptyMessage("Please log in to view your orders.")
            return
        }

        binding.llOrders.removeAllViews()

        lifecycleScope.launch {

            when (val result = CrustyRepository.getOrders(userId)) {

                is ApiResult.Success -> {

                    if (result.data.isEmpty()) {
                        showEmptyMessage("You have no orders yet.")
                        return@launch
                    }

                    val inflater = LayoutInflater.from(this@TrackerActivity)

                    result.data.forEach { order ->

                        val row = ItemOrderBinding.inflate(
                            inflater,
                            binding.llOrders,
                            false
                        )

                        row.tvOrderId.text =
                            getString(
                                R.string.tracker_order_number,
                                order.orderID
                            )

                        row.tvStatus.text = order.orderStatus

                        row.tvDate.text =
                            order.orderDate?.take(10) ?: ""

                        row.tvTotal.text =
                            getString(
                                R.string.price_format,
                                order.totalAmount.toInt()
                            )
                        val canCancel =
                            order.orderStatus.equals("Pending", ignoreCase = true) ||
                                    order.orderStatus.equals("Preparing", ignoreCase = true)

                        if (canCancel) {

                            row.btnCancelOrder.visibility = View.VISIBLE

                            row.btnCancelOrder.setOnClickListener {

                                lifecycleScope.launch {

                                    row.btnCancelOrder.isEnabled = false

                                    when (
                                        val cancelResult =
                                            CrustyRepository.cancelOrder(
                                                order.orderID,
                                                userId
                                            )
                                    ) {

                                        is ApiResult.Success -> {

                                            toast("Order cancelled.")

                                            loadOrders()
                                        }

                                        is ApiResult.Error -> {

                                            toast(cancelResult.message)

                                            row.btnCancelOrder.isEnabled = true
                                        }
                                    }
                                }
                            }

                        } else {

                            row.btnCancelOrder.visibility = View.GONE
                        }

                        row.tvItems.text =
                            order.items.joinToString(", ") {
                                "${it.itemName} ${
                                    getString(
                                        R.string.qty_format,
                                        it.quantity
                                    )
                                }"
                            }

                        binding.llOrders.addView(row.root)
                    }
                }

                is ApiResult.Error -> {
                    showEmptyMessage(result.message)
                }
            }
        }
    }

    private fun showEmptyMessage(message: String) {

        binding.llOrders.removeAllViews()

        val textView = TextView(this).apply {

            text = message

            setTextColor(0xFF8A8F9E.toInt())

            textSize = 15f

            gravity = android.view.Gravity.CENTER

            setPadding(
                16,
                48,
                16,
                48
            )
        }

        binding.llOrders.addView(textView)
    }
    private fun toast(message: String) {
        Toast.makeText(this, message, Toast.LENGTH_SHORT).show()
    }
}


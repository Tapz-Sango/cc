package com.example.crustycrab

import android.content.Intent
import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.lifecycleScope
import com.example.crustycrab.databinding.ActivityHomeBinding
import com.example.crustycrab.repository.CrustyRepository
import com.example.crustycrab.session.SessionManager
import com.example.crustycrab.util.ApiResult
import kotlinx.coroutines.launch

class HomeActivity : AppCompatActivity() {

    private lateinit var binding: ActivityHomeBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityHomeBinding.inflate(layoutInflater)
        setContentView(binding.root)

        // Greet the user from the session
        val name = SessionManager.getName(this).ifBlank { "Guest" }
        binding.tvWelcomeName.text = getString(R.string.welcome_message, name)

        // Quick action Browse Menu
        binding.tvBrowseTitle.setOnClickListener {
            startActivity(Intent(this, MenuActivity::class.java))
        }

        // Tap the Active Order card  Tracker
        binding.cardActiveOrder.setOnClickListener {
            startActivity(Intent(this, TrackerActivity::class.java))
        }

        setupBottomNavigation()
        loadActiveOrder()
    }

    override fun onResume() {
        super.onResume()
        // Refresh the Active Order card when returning to Home
        loadActiveOrder()
    }

    /**
     * Looks up the customer's most recent in-progress order.
     * If one exists, display it on the Active Order card. If not,
     * show the default "No Active Orders" state.
     */
    private fun loadActiveOrder() {
        val uid = SessionManager.getUserId(this)
        if (uid <= 0) return

        lifecycleScope.launch {
            when (val r = CrustyRepository.getOrders(uid)) {
                is ApiResult.Success -> {
                    val active = r.data.firstOrNull { order ->
                        val s = order.orderStatus.lowercase()
                        s == "pending" || s == "preparing" || s == "ready"
                    }
                    if (active != null) {
                        binding.tvOrderTitle.text = getString(
                            R.string.home_order_number,
                            active.orderID
                        )
                        binding.tvOrderSub.text = getString(
                            R.string.home_order_summary,
                            active.items.size,
                            active.totalAmount.toInt()
                        )
                    } else {
                        binding.tvOrderTitle.text =
                            getString(R.string.home_no_active_orders)
                        binding.tvOrderSub.text =
                            getString(R.string.home_no_active_orders_sub)
                    }
                }
                is ApiResult.Error -> {
                    binding.tvOrderTitle.text =
                        getString(R.string.home_no_active_orders)
                    binding.tvOrderSub.text =
                        getString(R.string.home_no_active_orders_sub)
                }
            }
        }
    }

    @Suppress("DEPRECATION")
    private fun setupBottomNavigation() {
        val nav = binding.bottomNavWrapper

        //  Home (current screen)
        nav.getChildAt(0).setOnClickListener {
            // Already on Home GÇö no-op
        }

        // 1 Menu
        nav.getChildAt(1).setOnClickListener {
            startActivity(Intent(this, MenuActivity::class.java))
            overridePendingTransition(0, 0)
        }

        // 2  Cart
        nav.getChildAt(2).setOnClickListener {
            startActivity(Intent(this, CartActivity::class.java))
            overridePendingTransition(0, 0)
        }

        // 3  Tracker
        nav.getChildAt(3).setOnClickListener {
            startActivity(Intent(this, TrackerActivity::class.java))
            overridePendingTransition(0, 0)
        }

        // 4  Account
        nav.getChildAt(4).setOnClickListener {
            startActivity(Intent(this, AccountActivity::class.java))
            overridePendingTransition(0, 0)
        }
    }
}

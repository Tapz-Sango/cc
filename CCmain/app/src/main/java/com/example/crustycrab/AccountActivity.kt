
package com.example.crustycrab

import android.content.Intent
import android.os.Bundle
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.example.crustycrab.databinding.ActivityAccountBinding
import com.example.crustycrab.session.SessionManager
import androidx.lifecycle.lifecycleScope
import com.example.crustycrab.repository.CrustyRepository
import com.example.crustycrab.util.ApiResult
import kotlinx.coroutines.launch

class AccountActivity : AppCompatActivity() {

    private lateinit var binding: ActivityAccountBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityAccountBinding.inflate(layoutInflater)
        setContentView(binding.root)

        // ---------- Profile (from session) ----------
        val name = SessionManager.getName(this).ifBlank { "Guest" }
        val email = SessionManager.getEmail(this).ifBlank { "not signed in" }

        val initials = name.split(" ")
            .mapNotNull { it.firstOrNull()?.uppercase() }
            .take(2)
            .joinToString("")

        binding.tvProfileName.text = name
        binding.tvProfileEmail.text = email
        binding.tvAvatar.text = initials.ifBlank { "?" }

        // Stat placeholders
        // Stats
        binding.tvOrdersCount.text = "0"
        binding.tvFavoritesCount.text = "-"
        binding.tvReviewsCount.text = "-"

        loadOrderCount()

        // ---------- Top bar back ----------
        binding.ivAccBack.setOnClickListener {
            finish()
        }

        // ---------- Menu items ----------
        binding.tvMenuOrderHistory.setOnClickListener {
            startActivity(Intent(this, TrackerActivity::class.java))
        }

        binding.tvMenuFavorites.setOnClickListener {
            toast("No favorites yet - favorite items will appear here.")
        }

        binding.tvMenuDeliveryAddress.setOnClickListener {
            startActivity(Intent(this, DeliveryAddressActivity::class.java))
        }

        binding.tvMenuPaymentMethods.setOnClickListener {
            startActivity(Intent(this, PaymentMethodsActivity::class.java))
        }

        binding.tvMenuNotifications.setOnClickListener {
            toast("Notifications coming soon.")
        }

        binding.tvMenuHelpSupport.setOnClickListener {
            startActivity(Intent(this, CrabbyBotActivity::class.java))
        }

        binding.tvMenuLogout.setOnClickListener {
            handleLogout()
        }

        setupBottomNavigation()
    }
    private fun loadOrderCount() {

        val userId = SessionManager.getUserId(this)

        if (userId <= 0) {
            binding.tvOrdersCount.text = "0"
            return
        }

        lifecycleScope.launch {

            when (val result = CrustyRepository.getOrders(userId)) {

                is ApiResult.Success -> {
                    binding.tvOrdersCount.text =
                        result.data.size.toString()
                }

                is ApiResult.Error -> {
                    binding.tvOrdersCount.text = "0"
                }
            }
        }
    }

    private fun handleLogout() {
        SessionManager.clear(this)

        toast("Logged out")

        startActivity(
            Intent(this, LoginActivity::class.java)
                .addFlags(
                    Intent.FLAG_ACTIVITY_NEW_TASK or
                            Intent.FLAG_ACTIVITY_CLEAR_TASK
                )
        )

        finish()
    }

    @Suppress("DEPRECATION")
    private fun setupBottomNavigation() {
        val nav = binding.bottomNavWrapper

        nav.getChildAt(0).setOnClickListener {
            startActivity(Intent(this, HomeActivity::class.java))
            finish()
            overridePendingTransition(0, 0)
        }

        nav.getChildAt(1).setOnClickListener {
            startActivity(Intent(this, MenuActivity::class.java))
            finish()
            overridePendingTransition(0, 0)
        }

        nav.getChildAt(2).setOnClickListener {
            startActivity(Intent(this, CartActivity::class.java))
            overridePendingTransition(0, 0)
        }

        nav.getChildAt(3).setOnClickListener {
            startActivity(Intent(this, TrackerActivity::class.java))
            finish()
            overridePendingTransition(0, 0)
        }

        nav.getChildAt(4).setOnClickListener {
            // Already on Account - no action needed
        }
    }

    private fun toast(message: String) {
        Toast.makeText(this, message, Toast.LENGTH_SHORT).show()
    }
}


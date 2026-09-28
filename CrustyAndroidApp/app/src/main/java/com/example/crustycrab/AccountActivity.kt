package com.example.crustycrab

import android.content.Intent
import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import com.example.crustycrab.databinding.ActivityAccountBinding

class AccountActivity : AppCompatActivity() {

    private lateinit var binding: ActivityAccountBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityAccountBinding.inflate(layoutInflater)
        setContentView(binding.root)

        binding.tvProfileName.text = getString(R.string.profile_name)
        binding.tvProfileEmail.text = getString(R.string.profile_email)
        binding.tvAvatar.text = getString(R.string.profile_initials)

        binding.tvOrdersCount.text = getString(R.string.profile_orders_count)
        binding.tvFavoritesCount.text = getString(R.string.profile_favorites_count)
        binding.tvReviewsCount.text = getString(R.string.profile_reviews_count)

        setupBottomNavigation()
    }

    @Suppress("DEPRECATION")
    private fun setupBottomNavigation() {
        binding.bottomNavWrapper.getChildAt(0).setOnClickListener {
            startActivity(Intent(this, HomeActivity::class.java))
            finish()
            overridePendingTransition(0, 0)
        }

        binding.bottomNavWrapper.getChildAt(1).setOnClickListener {
            startActivity(Intent(this, MenuActivity::class.java))
            finish()
            overridePendingTransition(0, 0)
        }
    }
}
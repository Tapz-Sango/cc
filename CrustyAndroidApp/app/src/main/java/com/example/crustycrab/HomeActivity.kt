package com.example.crustycrab

import android.content.Intent
import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import com.example.crustycrab.databinding.ActivityHomeBinding

class HomeActivity : AppCompatActivity() {

    private lateinit var binding: ActivityHomeBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityHomeBinding.inflate(layoutInflater)
        setContentView(binding.root)

        // Retrieve passed user email or default
        val userEmail = intent.getStringExtra("USER_EMAIL") ?: "James Bond"
        val displayName = userEmail.substringBefore("@").replaceFirstChar { it.uppercase() }

        binding.tvWelcomeName.text = getString(R.string.welcome_message, displayName)

        // Quick Actions setup
        binding.tvBrowseTitle.setOnClickListener {
            startActivity(Intent(this, MenuActivity::class.java))
        }

        setupBottomNavigation()
    }

    @Suppress("DEPRECATION")
    private fun setupBottomNavigation() {
        binding.bottomNavWrapper.getChildAt(1).setOnClickListener {
            startActivity(Intent(this, MenuActivity::class.java))
            overridePendingTransition(0, 0)
        }

        binding.bottomNavWrapper.getChildAt(4).setOnClickListener {
            startActivity(Intent(this, AccountActivity::class.java))
            overridePendingTransition(0, 0)
        }
    }
}
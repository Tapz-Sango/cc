package com.example.crustycrab

import android.content.Intent
import android.os.Bundle
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.example.crustycrab.databinding.ActivityMenuBinding

class MenuActivity : AppCompatActivity() {

    private lateinit var binding: ActivityMenuBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityMenuBinding.inflate(layoutInflater)
        setContentView(binding.root)

        binding.btnAdd1.setOnClickListener {
            Toast.makeText(this, "Cajun Shrimp Boil added to cart!", Toast.LENGTH_SHORT).show()
        }

        binding.btnAdd2.setOnClickListener {
            Toast.makeText(this, "Crab Feast added to cart!", Toast.LENGTH_SHORT).show()
        }

        setupBottomNavigation()
    }

    @Suppress("DEPRECATION")
    private fun setupBottomNavigation() {
        binding.bottomNavWrapper.getChildAt(0).setOnClickListener {
            startActivity(Intent(this, HomeActivity::class.java))
            finish()
            overridePendingTransition(0, 0)
        }

        binding.bottomNavWrapper.getChildAt(4).setOnClickListener {
            startActivity(Intent(this, AccountActivity::class.java))
            finish()
            overridePendingTransition(0, 0)
        }
    }
}
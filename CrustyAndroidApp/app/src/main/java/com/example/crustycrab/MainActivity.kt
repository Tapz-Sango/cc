package com.example.crustycrab

import android.content.Intent
import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity

class MainActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        // Route instantly to LoginActivity on launch
        val intent = Intent(this, LoginActivity::class.java)
        startActivity(intent)
        finish() // Removes MainActivity from the back stack
    }
}
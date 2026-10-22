package com.example.crustycrab

import android.os.Bundle
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.lifecycleScope
import com.example.crustycrab.api.dto.CreateSavedCardRequest
import com.example.crustycrab.databinding.ActivityAddPaymentMethodBinding
import com.example.crustycrab.repository.CrustyRepository
import com.example.crustycrab.session.SessionManager
import com.example.crustycrab.util.ApiResult
import kotlinx.coroutines.launch

class AddPaymentMethodActivity : AppCompatActivity() {

    private lateinit var binding: ActivityAddPaymentMethodBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        binding = ActivityAddPaymentMethodBinding.inflate(layoutInflater)
        setContentView(binding.root)

        binding.btnBack.setOnClickListener {
            finish()
        }

        binding.btnSavePaymentMethod.setOnClickListener {
            savePaymentMethod()
        }
    }

    private fun savePaymentMethod() {

        val customerId = SessionManager.getUserId(this)

        if (customerId <= 0) {
            toast("Please log in first.")
            return
        }

        val cardholderName =
            binding.etCardholderName.text.toString().trim()

        val cardNumber =
            binding.etCardNumber.text.toString().trim()

        val expiryMonth =
            binding.etExpiryMonth.text.toString().trim().toIntOrNull()

        val expiryYear =
            binding.etExpiryYear.text.toString().trim().toIntOrNull()

        if (cardholderName.isBlank()) {
            toast("Enter the cardholder name.")
            return
        }

        if (cardNumber.length !in 13..16) {
            toast("Enter a valid card number.")
            return
        }

        if (expiryMonth == null || expiryMonth !in 1..12) {
            toast("Enter a valid expiry month.")
            return
        }

        if (expiryYear == null || expiryYear < 2026) {
            toast("Enter a valid expiry year.")
            return
        }

        val request = CreateSavedCardRequest(
            customerID = customerId,
            paymentMethodID = 1,
            cardholderName = cardholderName,
            cardNumber = cardNumber,
            expiryMonth = expiryMonth,
            expiryYear = expiryYear
        )

        lifecycleScope.launch {

            binding.btnSavePaymentMethod.isEnabled = false

            when (val result =
                CrustyRepository.saveCustomerPaymentMethod(request)) {

                is ApiResult.Success -> {
                    toast("Payment method saved.")
                    finish()
                }

                is ApiResult.Error -> {
                    binding.btnSavePaymentMethod.isEnabled = true
                    toast(result.message)
                }
            }
        }
    }

    private fun toast(message: String) {
        Toast.makeText(this, message, Toast.LENGTH_SHORT).show()
    }
}
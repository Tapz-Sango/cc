
package com.example.crustycrab

import android.content.Intent
import android.os.Bundle
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.lifecycleScope
import com.example.crustycrab.api.dto.CreateCustomerRequest
import com.example.crustycrab.databinding.ActivitySignupBinding
import com.example.crustycrab.repository.CrustyRepository
import com.example.crustycrab.util.ApiResult
import kotlinx.coroutines.launch

class SignupActivity : AppCompatActivity() {

    private lateinit var binding: ActivitySignupBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        binding = ActivitySignupBinding.inflate(layoutInflater)
        setContentView(binding.root)

        binding.ivBackSignup.setOnClickListener {
            finish()
        }

        binding.btnSignupSubmit.setOnClickListener {

            val firstName = binding.etFirstName.text.toString().trim()
            val lastName = binding.etLastName.text.toString().trim()
            val email = binding.etSignupEmail.text.toString().trim()
            val phone = binding.etPhone.text.toString().trim()
            val password = binding.etSignupPassword.text.toString().trim()

            when {
                firstName.isEmpty() ||
                        lastName.isEmpty() ||
                        email.isEmpty() ||
                        phone.isEmpty() ||
                        password.isEmpty() -> {
                    toast("Please fill in all fields")
                }

                !email.contains("@") || !email.contains(".") -> {
                    toast("Enter a valid email address")
                }

                phone.length < 10 -> {
                    toast("Enter a valid phone number")
                }

                password.length < 6 -> {
                    toast("Password must be at least 6 characters")
                }

                else -> {
                    doSignup(
                        firstName,
                        lastName,
                        email,
                        phone,
                        password
                    )
                }
            }
        }

        binding.tvLoginPrompt.setOnClickListener {
            startActivity(
                Intent(this, LoginActivity::class.java)
            )
            finish()
        }
    }

    private fun doSignup(
        firstName: String,
        lastName: String,
        email: String,
        phone: String,
        password: String
    ) {
        setLoading(true)

        lifecycleScope.launch {

            val req = CreateCustomerRequest(
                firstName = firstName,
                lastName = lastName,
                phoneNumber = phone,
                emailAddress = email,
                password = password
            )

            when (val result = CrustyRepository.createCustomer(req)) {

                is ApiResult.Success -> {

                    toast("Account created! Please log in.")

                    startActivity(
                        Intent(
                            this@SignupActivity,
                            LoginActivity::class.java
                        )
                            .putExtra("USER_EMAIL", email)
                            .addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP)
                    )

                    finish()
                }

                is ApiResult.Error -> {
                    toast(result.message)
                    setLoading(false)
                }
            }
        }
    }

    private fun setLoading(loading: Boolean) {
        binding.btnSignupSubmit.isEnabled = !loading

        binding.btnSignupSubmit.text =
            if (loading) {
                "Creating account..."
            } else {
                "Create Account"
            }
    }

    private fun toast(message: String) {
        Toast.makeText(
            this,
            message,
            Toast.LENGTH_SHORT
        ).show()
    }
}


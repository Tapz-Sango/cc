package com.example.crustycrab

import android.content.Intent
import android.os.Bundle
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.lifecycleScope
import com.example.crustycrab.databinding.ActivityLoginBinding
import com.example.crustycrab.repository.CrustyRepository
import com.example.crustycrab.session.SessionManager
import com.example.crustycrab.util.ApiResult
import kotlinx.coroutines.launch

class LoginActivity : AppCompatActivity() {

    private lateinit var binding: ActivityLoginBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityLoginBinding.inflate(layoutInflater)
        setContentView(binding.root)

        binding.ivBack.setOnClickListener { finish() }

        binding.btnLoginSubmit.setOnClickListener {
            val email = binding.etEmail.text.toString().trim()
            val password = binding.etPassword.text.toString().trim()

            when {
                email.isEmpty() || password.isEmpty() ->
                    toast("Please fill in all fields")
                !email.contains("@") ->
                    toast("Enter a valid email address")
                password.length < 6 ->
                    toast("Password must be at least 6 characters")
                else -> doLogin(email, password)
            }
        }

        binding.tvForgotPassword.setOnClickListener {
            toast("Password reset link sent to your email.")
        }

        binding.tvSignUpPrompt.setOnClickListener {
            startActivity(Intent(this, SignupActivity::class.java))
        }
    }

    private fun doLogin(email: String, password: String) {
        setLoading(true)

        lifecycleScope.launch {
            val result = CrustyRepository.loginCustomer(email, password)

            when (result) {
                is ApiResult.Success -> {
                    val body = result.data

                    if (body.success) {
                        val displayName = body.username
                            .substringBefore("@")
                            .replaceFirstChar { it.uppercase() }

                        SessionManager.saveLogin(
                            ctx = this@LoginActivity,
                            userId = body.userId,
                            email = body.username,
                            name = displayName,
                            role = body.role
                        )

                        startActivity(
                            Intent(this@LoginActivity, HomeActivity::class.java)
                        )
                        finish()
                    } else {
                        // API returned 200, but success = false
                        toast(body.message.ifBlank { "Invalid email or password" })
                        setLoading(false)
                    }
                }

                is ApiResult.Error -> {
                    toast(result.message)
                    setLoading(false)
                }
            }
        }
    }

    private fun setLoading(loading: Boolean) {
        binding.btnLoginSubmit.isEnabled = !loading
        binding.btnLoginSubmit.text = if (loading) "Logging in..." else "Log In"
    }

    private fun toast(msg: String) =
        Toast.makeText(this, msg, Toast.LENGTH_SHORT).show()
}

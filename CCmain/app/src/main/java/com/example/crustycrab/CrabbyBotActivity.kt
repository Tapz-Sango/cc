package com.example.crustycrab

import android.os.Bundle
import android.view.Gravity
import android.view.View
import android.widget.LinearLayout
import android.widget.TextView
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.lifecycleScope
import com.example.crustycrab.api.dto.CrabbyBotOptionDto
import com.example.crustycrab.databinding.ActivityCrabbyBotBinding
import com.example.crustycrab.repository.CrustyRepository
import com.example.crustycrab.util.ApiResult
import kotlinx.coroutines.launch

class CrabbyBotActivity : AppCompatActivity() {

    private lateinit var binding: ActivityCrabbyBotBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityCrabbyBotBinding.inflate(layoutInflater)
        setContentView(binding.root)

        binding.btnBack.setOnClickListener { finish() }

        sendOption("start")
    }

    private fun sendOption(optionId: String) {
        lifecycleScope.launch {
            when (val r = CrustyRepository.crabbyBot(optionId)) {
                is ApiResult.Success -> {
                    addBotMessage(r.data.message)
                    renderOptions(r.data.options)
                }
                is ApiResult.Error -> addBotMessage("Error: ${r.message}")
            }
        }
    }

    private fun addBotMessage(text: String) {
        val tv = TextView(this).apply {
            this.text = text
            setTextColor(0xFFFFFFFF.toInt())
            textSize = 14f
            setBackgroundResource(R.drawable.bg_card_dark)
            setPadding(24, 24, 24, 24)
        }

        val lp = LinearLayout.LayoutParams(
            LinearLayout.LayoutParams.WRAP_CONTENT,
            LinearLayout.LayoutParams.WRAP_CONTENT
        ).apply {
            val d = resources.displayMetrics.density
            setMargins(0, (8 * d).toInt(), (64 * d).toInt(), (8 * d).toInt())
        }
        tv.layoutParams = lp

        binding.llMessages.addView(tv)

        // Auto-scroll to the newest message
        binding.svMessages.post {
            binding.svMessages.fullScroll(View.FOCUS_DOWN)
        }
    }

    private fun renderOptions(options: List<CrabbyBotOptionDto>) {
        binding.llOptions.removeAllViews()

        options.forEach { opt ->
            val chip = TextView(this).apply {
                text = opt.text
                setTextColor(0xFFFFFFFF.toInt())
                textSize = 13f
                setBackgroundResource(R.drawable.bg_chip_unselected)
                gravity = Gravity.CENTER
                setPadding(32, 20, 32, 20)
            }

            val lp = LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.WRAP_CONTENT,
                LinearLayout.LayoutParams.WRAP_CONTENT
            ).apply {
                val d = resources.displayMetrics.density
                setMargins((8 * d).toInt(), (8 * d).toInt(), (8 * d).toInt(), (8 * d).toInt())
            }
            chip.layoutParams = lp

            chip.setOnClickListener { sendOption(opt.id) }
            binding.llOptions.addView(chip)
        }
    }
}

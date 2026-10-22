package com.example.crustycrab

import android.content.Intent
import android.os.Bundle
import android.view.LayoutInflater
import android.widget.GridLayout
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.lifecycleScope
import com.bumptech.glide.Glide
import com.example.crustycrab.api.dto.CartDto
import com.example.crustycrab.api.dto.MenuItemDto
import com.example.crustycrab.databinding.ActivityMenuBinding
import com.example.crustycrab.databinding.ItemMenuItemBinding
import com.example.crustycrab.repository.CrustyRepository
import com.example.crustycrab.session.SessionManager
import com.example.crustycrab.util.ApiResult
import kotlinx.coroutines.launch
import com.example.crustycrab.api.ApiConfig

class MenuActivity : AppCompatActivity() {


    private lateinit var binding: ActivityMenuBinding

    private var allItems: List<MenuItemDto> = emptyList()
    private var currentFilter = "All"
    private var cart: CartDto? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        binding = ActivityMenuBinding.inflate(layoutInflater)
        setContentView(binding.root)

        setupFilters()
        setupCartButton()
        setupBottomNavigation()

        loadMenu()
        loadCart()
    }

    override fun onResume() {
        super.onResume()
        loadCart()
    }

    private fun setupFilters() {

        binding.llFilters.getChildAt(0).setOnClickListener {
            selectFilter("All")
        }

        binding.llFilters.getChildAt(1).setOnClickListener {
            selectFilter("Boil Bags")
        }

        binding.llFilters.getChildAt(2).setOnClickListener {
            selectFilter("Starters")
        }
    }

    private fun selectFilter(category: String) {

        currentFilter = category

        val selectedBg = R.drawable.bg_chip_selected
        val unselectedBg = R.drawable.bg_chip_unselected

        val selectedText = 0xFFFFFFFF.toInt()
        val unselectedText = 0xFF8A8F9E.toInt()

        for (i in 0 until binding.llFilters.childCount) {

            val chip = binding.llFilters.getChildAt(i)

            val isSelected = when (i) {
                0 -> category == "All"
                1 -> category == "Boil Bags"
                2 -> category == "Starters"
                else -> false
            }

            chip.setBackgroundResource(
                if (isSelected) selectedBg else unselectedBg
            )

            if (chip is android.widget.TextView) {
                chip.setTextColor(
                    if (isSelected) selectedText else unselectedText
                )
            }
        }

        renderMenu()
    }

    private fun loadMenu() {

        lifecycleScope.launch {

            when (val result = CrustyRepository.getMenu()) {

                is ApiResult.Success -> {

                    allItems = result.data.filter {
                        it.availability
                    }

                    renderMenu()
                }

                is ApiResult.Error -> {
                    toast(result.message)
                }
            }
        }
    }

    private fun renderMenu() {

        binding.menuGrid.removeAllViews()

        val filteredItems = if (currentFilter == "All") {
            allItems
        } else {
            allItems.filter {
                it.category.equals(
                    currentFilter,
                    ignoreCase = true
                )
            }
        }

        for (item in filteredItems) {

            val card = ItemMenuItemBinding.inflate(
                LayoutInflater.from(this),
                binding.menuGrid,
                false
            )

            card.tvItemName.text = item.itemName

            card.tvItemDesc.text =
                item.description ?: ""

            card.tvItemPrice.text =
                "R${item.price.toInt()}"

            val imageUrl = buildImageUrl(item.imageUrl)

            Glide.with(this)
                .load(imageUrl)
                .placeholder(R.drawable.app_logo)
                .error(R.drawable.app_logo)
                .into(card.ivItemImage)

            card.btnAdd.setOnClickListener {
                addItemToCart(item)
            }

            val params = GridLayout.LayoutParams().apply {
                width = 0
                height = GridLayout.LayoutParams.WRAP_CONTENT
                columnSpec =
                    GridLayout.spec(
                        GridLayout.UNDEFINED,
                        1f
                    )
                setMargins(8, 8, 8, 8)
            }

            card.root.layoutParams = params

            binding.menuGrid.addView(card.root)
        }
    }

    private fun buildImageUrl(path: String?): String? {

        if (path.isNullOrBlank()) {
            return null
        }

        return if (path.startsWith("http")) {
            path
        } else {
            ApiConfig.BASE_URL.trimEnd('/') +
                    if (path.startsWith("/")) {
                        path
                    } else {
                        "/$path"
                    }
        }
    }

    private fun setupCartButton() {

        binding.btnMenuCart.setOnClickListener {

            startActivity(
                Intent(
                    this,
                    CartActivity::class.java
                )
            )
        }
    }

    private fun loadCart() {

        val userId = SessionManager.getUserId(this)

        if (userId <= 0) {
            return
        }

        lifecycleScope.launch {

            when (
                val result =
                    CrustyRepository.getCart(userId)
            ) {

                is ApiResult.Success -> {

                    cart = result.data

                    updateCartButton()
                }

                is ApiResult.Error -> {

                    when (
                        val created =
                            CrustyRepository.createCart(userId)
                    ) {

                        is ApiResult.Success -> {

                            cart = created.data

                            updateCartButton()
                        }

                        is ApiResult.Error -> {
                            // Cart can be created later.
                        }
                    }
                }
            }
        }
    }

    private fun updateCartButton() {

        val count =
            cart?.items?.sumOf { it.quantity } ?: 0

        binding.btnMenuCart.text =
            "Cart ($count)"
    }

    private fun addItemToCart(item: MenuItemDto) {

        val userId =
            SessionManager.getUserId(this)

        if (userId <= 0) {

            toast("Please log in first.")

            return
        }

        val currentCart = cart

        if (currentCart == null) {

            lifecycleScope.launch {

                when (
                    val created =
                        CrustyRepository.createCart(userId)
                ) {

                    is ApiResult.Success -> {

                        cart = created.data

                        addItemToCart(item)
                    }

                    is ApiResult.Error -> {

                        toast("Cart is not ready yet.")
                    }
                }
            }

            return
        }

        lifecycleScope.launch {

            when (
                val result =
                    CrustyRepository.addToCart(
                        currentCart.cartId,
                        item.menuItemId,
                        1
                    )
            ) {

                is ApiResult.Success -> {

                    cart = result.data

                    updateCartButton()

                    toast(
                        "${item.itemName} added to cart"
                    )
                }

                is ApiResult.Error -> {

                    toast(result.message)
                }
            }
        }
    }

    @Suppress("DEPRECATION")
    private fun setupBottomNavigation() {

        val nav = binding.bottomNavWrapper

        nav.getChildAt(0).setOnClickListener {

            startActivity(
                Intent(
                    this,
                    HomeActivity::class.java
                )
            )

            finish()

            overridePendingTransition(0, 0)
        }

        nav.getChildAt(1).setOnClickListener {
            // Already on Menu
        }

        nav.getChildAt(2).setOnClickListener {

            startActivity(
                Intent(
                    this,
                    CartActivity::class.java
                )
            )

            overridePendingTransition(0, 0)
        }

        nav.getChildAt(3).setOnClickListener {

            startActivity(
                Intent(
                    this,
                    TrackerActivity::class.java
                )
            )

            finish()

            overridePendingTransition(0, 0)
        }

        nav.getChildAt(4).setOnClickListener {

            startActivity(
                Intent(
                    this,
                    AccountActivity::class.java
                )
            )

            finish()

            overridePendingTransition(0, 0)
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

package com.example.crustycrab

import android.app.AlertDialog
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.widget.EditText
import android.widget.LinearLayout
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.lifecycleScope
import com.example.crustycrab.api.dto.CreateDeliveryAddressRequest
import com.example.crustycrab.api.dto.DeliveryAddressDto
import com.example.crustycrab.api.dto.UpdateDeliveryAddressRequest
import com.example.crustycrab.databinding.ActivityDeliveryAddressBinding
import com.example.crustycrab.databinding.ItemAddressBinding
import com.example.crustycrab.repository.CrustyRepository
import com.example.crustycrab.session.SessionManager
import com.example.crustycrab.util.ApiResult
import kotlinx.coroutines.launch

class DeliveryAddressActivity : AppCompatActivity() {

    private lateinit var binding: ActivityDeliveryAddressBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityDeliveryAddressBinding.inflate(layoutInflater)
        setContentView(binding.root)

        binding.btnBack.setOnClickListener { finish() }
        binding.btnAddAddress.setOnClickListener { showAddressDialog(null) }

        loadAddresses()
    }

    private fun loadAddresses() {
        val uid = SessionManager.getUserId(this)
        if (uid <= 0) {
            toast(getString(R.string.login_required))
            return
        }

        lifecycleScope.launch {
            when (val r = CrustyRepository.getAddresses(uid)) {
                is ApiResult.Success -> render(r.data)
                is ApiResult.Error -> toast(r.message)
            }
        }
    }

    private fun render(list: List<DeliveryAddressDto>) {
        binding.llAddresses.removeAllViews()

        if (list.isEmpty()) {
            binding.tvEmpty.visibility = View.VISIBLE
            return
        }

        binding.tvEmpty.visibility = View.GONE

        val inflater = LayoutInflater.from(this)
        list.forEach { addr ->
            val row = ItemAddressBinding.inflate(inflater, binding.llAddresses, false)
            row.tvLine1.text = "${addr.houseNumber} ${addr.streetName}"
            row.tvLine2.text = "${addr.suburb} (${addr.buildingType ?: "Residential"})"
            row.btnEdit.setOnClickListener { showAddressDialog(addr) }
            row.btnDelete.setOnClickListener { deleteAddress(addr.deliveryAddressId) }
            binding.llAddresses.addView(row.root)
        }
    }

    private fun showAddressDialog(existing: DeliveryAddressDto?) {
        val container = LinearLayout(this).apply {
            orientation = LinearLayout.VERTICAL
            setPadding(32, 16, 32, 16)
        }

        val etHouse = EditText(this).apply {
            hint = "House Number"
            setText(existing?.houseNumber ?: "")
        }
        val etStreet = EditText(this).apply {
            hint = "Street Name"
            setText(existing?.streetName ?: "")
        }
        val etSuburb = EditText(this).apply {
            hint = "Suburb"
            setText(existing?.suburb ?: "")
        }
        val etBuilding = EditText(this).apply {
            hint = "Building Type (optional)"
            setText(existing?.buildingType ?: "")
        }

        container.addView(etHouse)
        container.addView(etStreet)
        container.addView(etSuburb)
        container.addView(etBuilding)

        AlertDialog.Builder(this)
            .setTitle(if (existing == null) "Add Address" else "Edit Address")
            .setView(container)
            .setPositiveButton("Save") { _, _ ->
                val house = etHouse.text.toString().trim()
                val street = etStreet.text.toString().trim()
                val suburb = etSuburb.text.toString().trim()
                val building = etBuilding.text.toString().trim().ifBlank { null }

                when {
                    house.isEmpty() || street.isEmpty() || suburb.isEmpty() ->
                        toast("House, street, and suburb are required.")
                    existing == null -> createAddress(suburb, house, street, building)
                    else -> updateAddress(existing.deliveryAddressId, suburb, house, street, building)
                }
            }
            .setNegativeButton("Cancel", null)
            .show()
    }

    private fun createAddress(
        suburb: String,
        house: String,
        street: String,
        building: String?
    ) {
        val uid = SessionManager.getUserId(this)
        val req = CreateDeliveryAddressRequest(
            suburb = suburb,
            houseNumber = house,
            streetName = street,
            buildingType = building,
            customerId = uid
        )

        lifecycleScope.launch {
            when (val r = CrustyRepository.createAddress(req)) {
                is ApiResult.Success -> {
                    toast("Address added")
                    loadAddresses()
                }
                is ApiResult.Error -> toast(r.message)
            }
        }
    }

    private fun updateAddress(
        id: Int,
        suburb: String,
        house: String,
        street: String,
        building: String?
    ) {
        val req = UpdateDeliveryAddressRequest(
            suburb = suburb,
            houseNumber = house,
            streetName = street,
            buildingType = building
        )

        lifecycleScope.launch {
            when (val r = CrustyRepository.updateAddress(id, req)) {
                is ApiResult.Success -> {
                    toast("Address updated")
                    loadAddresses()
                }
                is ApiResult.Error -> toast(r.message)
            }
        }
    }

    private fun deleteAddress(id: Int) {
        AlertDialog.Builder(this)
            .setTitle("Delete Address")
            .setMessage("Are you sure?")
            .setPositiveButton("Delete") { _, _ ->
                lifecycleScope.launch {
                    when (val r = CrustyRepository.deleteAddress(id)) {
                        is ApiResult.Success -> {
                            toast("Deleted")
                            loadAddresses()
                        }
                        is ApiResult.Error -> toast(r.message)
                    }
                }
            }
            .setNegativeButton("Cancel", null)
            .show()
    }

    private fun toast(m: String) =
        Toast.makeText(this, m, Toast.LENGTH_SHORT).show()
}

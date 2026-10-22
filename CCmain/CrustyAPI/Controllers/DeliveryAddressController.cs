using CrustyAPI.DTOs;
using CrustyAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrustyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeliveryAddressController : ControllerBase
{
    private readonly DeliveryAddressService _addressService;

    public DeliveryAddressController(DeliveryAddressService addressService)
    {
        _addressService = addressService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAddresses()
    {
        var addresses = await _addressService.GetAllAddresses();

        return Ok(addresses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAddress(int id)
    {
        var address = await _addressService.GetAddressById(id);

        if (address == null)
        {
            return NotFound(new
            {
                message = "Delivery address not found"
            });
        }

        return Ok(address);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetCustomerAddresses(int customerId)
    {
        var addresses = await _addressService.GetAddressesByCustomer(customerId);

        return Ok(addresses);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAddress(
        CreateDeliveryAddressRequest request)
    {
        var address = await _addressService.CreateAddress(request);

        if (address == null)
        {
            return NotFound(new
            {
                message = "Customer not found"
            });
        }

        return CreatedAtAction(
            nameof(GetAddress),
            new { id = address.DeliveryAddressID },
            address);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAddress(
        int id,
        UpdateDeliveryAddressRequest request)
    {
        var address = await _addressService.UpdateAddress(id, request);

        if (address == null)
        {
            return NotFound(new
            {
                message = "Delivery address not found"
            });
        }

        return Ok(address);
    }




    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        try
        {
            var deleted = await _addressService.DeleteAddress(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Delivery address not found"
                });
            }

            return NoContent();
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            when (ex.InnerException is MySqlConnector.MySqlException mysqlEx
                  && mysqlEx.Number == 1451)
        {
            return Conflict(new
            {
                message = "This address cannot be deleted because it is associated with an existing order."
            });
        }
        catch (MySqlConnector.MySqlException ex) when (ex.Number == 1451)
        {
            return Conflict(new
            {
                message = "This address cannot be deleted because it is associated with an existing order."
            });
        }
    }
}
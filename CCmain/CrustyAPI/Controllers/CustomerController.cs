using CrustyAPI.DTOs;
using CrustyAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrustyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly CustomerService _customerService;

    public CustomerController(CustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers()
    {
        var customers = await _customerService.GetAllCustomers();

        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomer(int id)
    {
        var customer = await _customerService.GetCustomerById(id);

        if (customer == null)
        {
            return NotFound(new
            {
                message = "Customer not found"
            });
        }

        return Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer(
        CreateCustomerRequest request)
    {
        var customer = await _customerService.CreateCustomer(request);

        if (customer == null)
        {
            return Conflict(new
            {
                message = "A customer with this email already exists."
            });
        }

        return CreatedAtAction(
            nameof(GetCustomer),
            new { id = customer.CustomerID },
            customer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(
        int id,
        UpdateCustomerRequest request)
    {
        var customer = await _customerService.UpdateCustomer(id, request);

        if (customer == null)
        {
            return NotFound(new
            {
                message = "Customer not found or email is already in use."
            });
        }

        return Ok(customer);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var deleted = await _customerService.DeleteCustomer(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Customer not found"
            });
        }

        return NoContent();
    }
}
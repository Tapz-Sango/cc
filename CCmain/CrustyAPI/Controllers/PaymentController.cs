using CrustyAPI.DTOs;
using CrustyAPI.Services;
using Microsoft.AspNetCore.Mvc;
using static CrustyAPI.DTOs.PaymentResponse;

namespace CrustyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly PaymentService _paymentService;

    public PaymentController(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    // =========================================================
    // PAYMENT METHODS
    // =========================================================

    [HttpGet("methods")]
    public async Task<IActionResult> GetPaymentMethods()
    {
        var methods = await _paymentService.GetPaymentMethods();

        return Ok(methods);
    }

    [HttpGet("methods/{id}")]
    public async Task<IActionResult> GetPaymentMethod(int id)
    {
        var method = await _paymentService.GetPaymentMethodById(id);

        if (method == null)
        {
            return NotFound(new { message = "Payment method not found" });
        }

        return Ok(method);
    }

    [HttpPost("methods")]
    public async Task<IActionResult> CreatePaymentMethod(
        CreatePaymentMethodRequest request)
    {
        var method = await _paymentService.CreatePaymentMethod(request);

        return CreatedAtAction(
            nameof(GetPaymentMethod),
            new { id = method.PaymentMethodID },
            method
        );
    }

    [HttpDelete("methods/{id}")]
    public async Task<IActionResult> DeletePaymentMethod(int id)
    {
        var deleted = await _paymentService.DeletePaymentMethod(id);

        if (!deleted)
        {
            return NotFound(new { message = "Payment method not found" });
        }

        return NoContent();
    }

    // =========================================================
    // CUSTOMER SAVED PAYMENT METHODS
    // =========================================================

    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetCustomerPaymentMethods(
        int customerId)
    {
        var methods =
            await _paymentService.GetCustomerPaymentMethods(customerId);

        return Ok(methods);
    }

    [HttpPost("customer")]
    public async Task<IActionResult> SaveCustomerPaymentMethod(
        CreateSavedCardRequest request)
    {
        var method =
            await _paymentService.SaveCustomerPaymentMethod(request);

        if (method == null)
        {
            return BadRequest(new
            {
                message = "Invalid customer or payment method."
            });
        }

        return Ok(method);
    }

    [HttpDelete("customer/{id}")]
    public async Task<IActionResult> DeleteCustomerPaymentMethod(
        int id,
        [FromQuery] int customerId)
    {
        var deleted =
            await _paymentService.DeleteCustomerPaymentMethod(
                id,
                customerId);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Saved payment method not found."
            });
        }

        return NoContent();
    }

    // =========================================================
    // PAYMENTS
    // =========================================================

    [HttpPost]
    public async Task<IActionResult> CreatePayment(
        CreatePaymentRequest request)
    {
        var payment = await _paymentService.CreatePayment(request);

        if (payment == null)
        {
            return BadRequest(new
            {
                message = "Invalid order or payment method."
            });
        }

        return CreatedAtAction(
            nameof(GetPayment),
            new { id = payment.PaymentID },
            payment
        );
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPayment(int id)
    {
        var payment = await _paymentService.GetPaymentById(id);

        if (payment == null)
        {
            return NotFound(new { message = "Payment not found" });
        }

        return Ok(payment);
    }

    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetOrderPayments(int orderId)
    {
        var payments = await _paymentService.GetPaymentsByOrder(orderId);

        return Ok(payments);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdatePaymentStatus(
        int id,
        UpdatePaymentStatusRequest request)
    {
        var payment = await _paymentService.UpdatePaymentStatus(
            id,
            request);

        if (payment == null)
        {
            return NotFound(new { message = "Payment not found" });
        }

        return Ok(payment);
    }
}
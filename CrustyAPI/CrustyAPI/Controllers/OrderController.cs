using CrustyAPI.DTOs;
using CrustyAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrustyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrderController(OrderService orderService)
    {
        _orderService = orderService;
    }

    // GET: api/order
    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _orderService.GetAllOrders();

        return Ok(orders);
    }

    // GET: api/order/1
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var order = await _orderService.GetOrderById(id);

        if (order == null)
        {
            return NotFound(new { message = "Order not found" });
        }

        return Ok(order);
    }

    // GET: api/order/customer/1
    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetCustomerOrders(int customerId)
    {
        var orders = await _orderService.GetOrdersByCustomer(customerId);

        return Ok(orders);
    }

    // POST: api/order
    [HttpPost]
    public async Task<IActionResult> CreateOrder(
        CreateOrderRequest request)
    {
        var order = await _orderService.CreateOrder(request);

        if (order == null)
        {
            return BadRequest(new
            {
                message = "Invalid customer, delivery address, menu item, or order."
            });
        }

        return CreatedAtAction(
            nameof(GetOrder),
            new { id = order.OrderID },
            order
        );
    }

    // PUT: api/order/1/cancel?customerId=1
    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(
        int id,
        [FromQuery] int customerId)
    {
        var order = await _orderService.CancelOrder(
            id,
            customerId);

        if (order == null)
        {
            return BadRequest(new
            {
                message = "Order cannot be cancelled."
            });
        }

        return Ok(order);
    }

    // PUT: api/order/1/status
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(
        int id,
        UpdateOrderStatusRequest request)
    {
        var order = await _orderService.UpdateOrderStatus(
            id,
            request);

        if (order == null)
        {
            return NotFound(new { message = "Order not found" });
        }

        return Ok(order);
    }
}
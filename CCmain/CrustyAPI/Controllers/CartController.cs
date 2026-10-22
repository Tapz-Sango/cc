using CrustyAPI.DTOs;
using CrustyAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrustyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly CartService _cartService;

    public CartController(CartService cartService)
    {
        _cartService = cartService;
    }

    // GET: api/cart/customer/1
    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetCustomerCart(int customerId)
    {
        var cart = await _cartService.GetCartByCustomer(customerId);

        if (cart == null)
        {
            return NotFound(new { message = "Cart not found" });
        }

        return Ok(cart);
    }

    // GET: api/cart
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCart(int id)
    {
        var cart = await _cartService.GetCartById(id);

        if (cart == null)
        {
            return NotFound(new { message = "Cart not found" });
        }

        return Ok(cart);
    }

    // POST: api/cart
    [HttpPost]
    public async Task<IActionResult> CreateCart(CreateCartRequest request)
    {
        var cart = await _cartService.CreateCart(request);

        if (cart == null)
        {
            return NotFound(new { message = "Customer not found" });
        }

        return Ok(cart);
    }

    // POST: api/cart/1/items
    [HttpPost("{cartId}/items")]
    public async Task<IActionResult> AddItem(
        int cartId,
        AddCartItemRequest request)
    {
        var cart = await _cartService.AddItemToCart(cartId, request);

        if (cart == null)
        {
            return BadRequest(new
            {
                message = "Cart not found, menu item not found, or menu item is unavailable."
            });
        }

        return Ok(cart);
    }

    // PUT: api/cart/items/1
    [HttpPut("items/{cartItemId}")]
    public async Task<IActionResult> UpdateItem(
        int cartItemId,
        UpdateCartItemRequest request)
    {
        var cart = await _cartService.UpdateCartItem(
            cartItemId,
            request);

        if (cart == null)
        {
            return NotFound(new { message = "Cart item not found" });
        }

        return Ok(cart);
    }

    // DELETE: api/cart/items/1
    [HttpDelete("items/{cartItemId}")]
    public async Task<IActionResult> RemoveItem(int cartItemId)
    {
        var cart = await _cartService.RemoveCartItem(cartItemId);

        if (cart == null)
        {
            return NotFound(new { message = "Cart item not found" });
        }

        return Ok(cart);
    }
}
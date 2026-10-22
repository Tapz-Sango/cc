using CrustyAPI.Data;
using CrustyAPI.DTOs;
using CrustyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CrustyAPI.Services;

public class CartService
{
    private readonly AppDbContext _context;

    public CartService(AppDbContext context)
    {
        _context = context;
    }

    // Get a customer's cart
    public async Task<CartResponse?> GetCartByCustomer(int customerId)
    {
        var cart = await _context.Carts
            .FirstOrDefaultAsync(c => c.CustomerID == customerId);

        if (cart == null)
        {
            return null;
        }

        return await BuildCartResponse(cart);
    }

    // Create a new cart for a customer
    public async Task<CartResponse?> CreateCart(CreateCartRequest request)
    {
        var customerExists = await _context.Customers
            .AnyAsync(c => c.CustomerID == request.CustomerID);

        if (!customerExists)
        {
            return null;
        }

        // Don't allow multiple carts for the same customer
        var existingCart = await _context.Carts
            .FirstOrDefaultAsync(c => c.CustomerID == request.CustomerID);

        if (existingCart != null)
        {
            return await BuildCartResponse(existingCart);
        }

        var cart = new Cart
        {
            CustomerID = request.CustomerID,
            CreatedDate = DateTime.UtcNow,
            TotalAmount = 0
        };

        _context.Carts.Add(cart);

        await _context.SaveChangesAsync();

        return new CartResponse
        {
            CartID = cart.CartID,
            CustomerID = cart.CustomerID,
            CreatedDate = cart.CreatedDate,
            TotalAmount = cart.TotalAmount,
            Items = new List<CartItemResponse>()
        };
    }

    // Add an item to the cart
    public async Task<CartResponse?> AddItemToCart(
        int cartId,
        AddCartItemRequest request)
    {
        var cart = await _context.Carts
            .FirstOrDefaultAsync(c => c.CartID == cartId);

        if (cart == null)
        {
            return null;
        }

        var menuItem = await _context.MenuItems
            .FirstOrDefaultAsync(m =>
                m.MenuItemID == request.MenuItemID);

        if (menuItem == null)
        {
            return null;
        }

        if (!menuItem.Availability)
        {
            return null;
        }

        var existingCartItem = await _context.CartItems
            .FirstOrDefaultAsync(ci =>
                ci.CartID == cartId &&
                ci.MenuItemID == request.MenuItemID);

        if (existingCartItem != null)
        {
            existingCartItem.Quantity += request.Quantity;
        }
        else
        {
            var cartItem = new CartItem
            {
                CartID = cartId,
                MenuItemID = menuItem.MenuItemID,
                ItemPrice = menuItem.Price,
                Quantity = request.Quantity
            };

            _context.CartItems.Add(cartItem);
        }

        await _context.SaveChangesAsync();

        await RecalculateCartTotal(cartId);

        return await GetCartById(cartId);
    }

    // Update quantity of an item
    public async Task<CartResponse?> UpdateCartItem(
        int cartItemId,
        UpdateCartItemRequest request)
    {
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.CartItemID == cartItemId);

        if (cartItem == null)
        {
            return null;
        }

        cartItem.Quantity = request.Quantity;

        await _context.SaveChangesAsync();

        await RecalculateCartTotal(cartItem.CartID);

        return await GetCartById(cartItem.CartID);
    }

    // Remove an item from the cart
    public async Task<CartResponse?> RemoveCartItem(int cartItemId)
    {
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.CartItemID == cartItemId);

        if (cartItem == null)
        {
            return null;
        }

        var cartId = cartItem.CartID;

        _context.CartItems.Remove(cartItem);

        await _context.SaveChangesAsync();

        await RecalculateCartTotal(cartId);

        return await GetCartById(cartId);
    }

    // Get cart by ID
    public async Task<CartResponse?> GetCartById(int cartId)
    {
        var cart = await _context.Carts
            .FirstOrDefaultAsync(c => c.CartID == cartId);

        if (cart == null)
        {
            return null;
        }

        return await BuildCartResponse(cart);
    }

    // Recalculate total amount
    private async Task RecalculateCartTotal(int cartId)
    {
        var total = await _context.CartItems
            .Where(ci => ci.CartID == cartId)
            .SumAsync(ci => ci.ItemPrice * ci.Quantity);

        var cart = await _context.Carts
            .FirstOrDefaultAsync(c => c.CartID == cartId);

        if (cart != null)
        {
            cart.TotalAmount = total;

            await _context.SaveChangesAsync();
        }
    }

    // Build the complete cart response
    private async Task<CartResponse> BuildCartResponse(Cart cart)
    {
        var items = await _context.CartItems
            .Where(ci => ci.CartID == cart.CartID)
            .Join(
                _context.MenuItems,
                cartItem => cartItem.MenuItemID,
                menuItem => menuItem.MenuItemID,
                (cartItem, menuItem) => new CartItemResponse
                {
                    CartItemID = cartItem.CartItemID,
                    MenuItemID = cartItem.MenuItemID,
                    ItemName = menuItem.ItemName,
                    ItemPrice = cartItem.ItemPrice,
                    Quantity = cartItem.Quantity,
                    Subtotal = cartItem.ItemPrice * cartItem.Quantity
                })
            .ToListAsync();

        return new CartResponse
        {
            CartID = cart.CartID,
            CustomerID = cart.CustomerID,
            CreatedDate = cart.CreatedDate,
            TotalAmount = cart.TotalAmount,
            Items = items
        };
    }
}
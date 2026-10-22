using CrustyAPI.Data;
using CrustyAPI.DTOs;
using CrustyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CrustyAPI.Services;

public class OrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    // Create a new order
    public async Task<OrderResponse?> CreateOrder(CreateOrderRequest request)
    {
        // Check customer
        var customerExists = await _context.Customers
            .AnyAsync(c => c.CustomerID == request.CustomerID);

        if (!customerExists)
        {
            return null;
        }

        // Check delivery address if one was supplied
        if (request.DeliveryAddressID.HasValue)
        {
            var addressExists = await _context.DeliveryAddresses
                .AnyAsync(a =>
                    a.DeliveryAddressID == request.DeliveryAddressID.Value &&
                    a.CustomerID == request.CustomerID);

            if (!addressExists)
            {
                return null;
            }
        }

        // Make sure the order contains items
        if (request.Items == null || request.Items.Count == 0)
        {
            return null;
        }

        var order = new Order
        {
            CustomerID = request.CustomerID,
            DeliveryAddressID = request.DeliveryAddressID,
            DeliveryMethod = request.DeliveryMethod,
            OrderStatus = "Pending",
            OrderDate = DateTime.UtcNow,
            TotalAmount = 0
        };

        _context.Orders.Add(order);

        await _context.SaveChangesAsync();

        decimal total = 0;

        foreach (var requestItem in request.Items)
        {
            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(m =>
                    m.MenuItemID == requestItem.MenuItemID);

            if (menuItem == null || !menuItem.Availability)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                return null;
            }

            var orderItem = new OrderItem
            {
                OrderID = order.OrderID,
                MenuItemID = menuItem.MenuItemID,
                Quantity = requestItem.Quantity,
                ItemPrice = menuItem.Price,
                Notes = requestItem.Notes
            };

            _context.OrderItems.Add(orderItem);

            total += menuItem.Price * requestItem.Quantity;
        }

        order.TotalAmount = total;

        await _context.SaveChangesAsync();

        return await BuildOrderResponse(order);
    }

    // Get order by ID
    public async Task<OrderResponse?> GetOrderById(int orderId)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.OrderID == orderId);

        if (order == null)
        {
            return null;
        }

        return await BuildOrderResponse(order);
    }

    // Get all orders for a customer
    public async Task<List<OrderResponse>> GetOrdersByCustomer(int customerId)
    {
        var orders = await _context.Orders
            .Where(o => o.CustomerID == customerId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        var responses = new List<OrderResponse>();

        foreach (var order in orders)
        {
            responses.Add(await BuildOrderResponse(order));
        }

        return responses;
    }

    // Get all orders
    public async Task<List<OrderResponse>> GetAllOrders()
    {
        var orders = await _context.Orders
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        var responses = new List<OrderResponse>();

        foreach (var order in orders)
        {
            responses.Add(await BuildOrderResponse(order));
        }

        return responses;
    }

    // Update order status
    public async Task<OrderResponse?> UpdateOrderStatus(
        int orderId,
        UpdateOrderStatusRequest request)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.OrderID == orderId);

        if (order == null)
        {
            return null;
        }

        order.OrderStatus = request.OrderStatus;

        await _context.SaveChangesAsync();

        return await BuildOrderResponse(order);
    }
    // Cancel an order
    public async Task<OrderResponse?> CancelOrder(
        int orderId,
        int customerId)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o =>
                o.OrderID == orderId &&
                o.CustomerID == customerId);

        if (order == null)
        {
            return null;
        }

        // Customers can only cancel pending orders
        if (order.OrderStatus != "Pending" &&
    order.OrderStatus != "Preparing")
        {
            return null;
        }

        order.OrderStatus = "Cancelled";

        await _context.SaveChangesAsync();

        return await BuildOrderResponse(order);
    }

    // Build complete order response
    private async Task<OrderResponse> BuildOrderResponse(Order order)
    {
        var items = await _context.OrderItems
            .Where(oi => oi.OrderID == order.OrderID)
            .Join(
                _context.MenuItems,
                orderItem => orderItem.MenuItemID,
                menuItem => menuItem.MenuItemID,
                (orderItem, menuItem) => new OrderItemResponse
                {
                    OrderItemID = orderItem.OrderItemID,
                    MenuItemID = orderItem.MenuItemID,
                    ItemName = menuItem.ItemName,
                    Quantity = orderItem.Quantity,
                    ItemPrice = orderItem.ItemPrice,
                    Subtotal = orderItem.ItemPrice * orderItem.Quantity,
                    Notes = orderItem.Notes
                })
            .ToListAsync();

        return new OrderResponse
        {
            OrderID = order.OrderID,
            OrderDate = order.OrderDate,
            OrderStatus = order.OrderStatus,
            DeliveryMethod = order.DeliveryMethod,
            TotalAmount = order.TotalAmount,
            DeliveryAddressID = order.DeliveryAddressID,
            CustomerID = order.CustomerID,
            StaffId = order.StaffId,
            Items = items
        };
    }
}
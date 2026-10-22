using CrustyAPI.Data;
using CrustyAPI.Models;
using CrustyAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace CrustyAPI.Tests;

public class OrderServiceTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CancelOrder_PendingOrder_ChangesStatusToCancelled()
    {
        // Arrange
        await using var context = CreateContext();

        var order = new Order
        {
            CustomerID = 1,
            OrderStatus = "Pending",
            DeliveryMethod = "Delivery",
            TotalAmount = 100
        };

        context.Orders.Add(order);
        await context.SaveChangesAsync();

        var service = new OrderService(context);

        // Act
        var result = await service.CancelOrder(order.OrderID, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Cancelled", result.OrderStatus);
    }

    [Fact]
    public async Task CancelOrder_PreparingOrder_ChangesStatusToCancelled()
    {
        // Arrange
        await using var context = CreateContext();

        var order = new Order
        {
            CustomerID = 1,
            OrderStatus = "Preparing",
            DeliveryMethod = "Delivery",
            TotalAmount = 100
        };

        context.Orders.Add(order);
        await context.SaveChangesAsync();

        var service = new OrderService(context);

        // Act
        var result = await service.CancelOrder(order.OrderID, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Cancelled", result.OrderStatus);
    }

    [Fact]
    public async Task CancelOrder_CompletedOrder_ReturnsNull()
    {
        // Arrange
        await using var context = CreateContext();

        var order = new Order
        {
            CustomerID = 1,
            OrderStatus = "Completed",
            DeliveryMethod = "Delivery",
            TotalAmount = 100
        };

        context.Orders.Add(order);
        await context.SaveChangesAsync();

        var service = new OrderService(context);

        // Act
        var result = await service.CancelOrder(order.OrderID, 1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CancelOrder_WrongCustomer_ReturnsNull()
    {
        // Arrange
        await using var context = CreateContext();

        var order = new Order
        {
            CustomerID = 1,
            OrderStatus = "Pending",
            DeliveryMethod = "Delivery",
            TotalAmount = 100
        };

        context.Orders.Add(order);
        await context.SaveChangesAsync();

        var service = new OrderService(context);

        // Act
        var result = await service.CancelOrder(order.OrderID, 999);

        // Assert
        Assert.Null(result);
    }
}
using CrustyAPI.Data;
using CrustyAPI.DTOs;
using CrustyAPI.Models;
using CrustyAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace CrustyAPI.Tests;

public class CartTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateCart_ValidCustomer_CreatesCart()
    {
        // Arrange
        await using var context = CreateContext();

        context.Customers.Add(new Customer
        {
            CustomerID = 1,
            FirstName = "Test",
            LastName = "Customer",
            PhoneNumber = "0123456789",
            EmailAddress = "test@test.com",
            PasswordHash = "hashed"
        });

        await context.SaveChangesAsync();

        var service = new CartService(context);

        var request = new CreateCartRequest
        {
            CustomerID = 1
        };

        // Act
        var result = await service.CreateCart(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.CustomerID);
        Assert.Equal(0, result.TotalAmount);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task CreateCart_InvalidCustomer_ReturnsNull()
    {
        // Arrange
        await using var context = CreateContext();

        var service = new CartService(context);

        var request = new CreateCartRequest
        {
            CustomerID = 999
        };

        // Act
        var result = await service.CreateCart(request);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddItemToCart_ValidItem_CalculatesCorrectTotal()
    {
        // Arrange
        await using var context = CreateContext();

        var customer = new Customer
        {
            CustomerID = 1,
            FirstName = "Test",
            LastName = "Customer",
            PhoneNumber = "0123456789",
            EmailAddress = "test@test.com",
            PasswordHash = "hashed"
        };

        var cart = new Cart
        {
            CartID = 1,
            CustomerID = 1,
            CreatedDate = DateTime.UtcNow,
            TotalAmount = 0
        };

        var menuItem = new MenuItem
        {
            MenuItemID = 1,
            ItemName = "Test Burger",
            Description = "Test item",
            Price = 50,
            Category = "Burgers",
            Availability = true
        };

        context.Customers.Add(customer);
        context.Carts.Add(cart);
        context.MenuItems.Add(menuItem);

        await context.SaveChangesAsync();

        var service = new CartService(context);

        var request = new AddCartItemRequest
        {
            MenuItemID = 1,
            Quantity = 2
        };

        // Act
        var result = await service.AddItemToCart(1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100, result.TotalAmount);
        Assert.Single(result.Items);
        Assert.Equal(2, result.Items[0].Quantity);
    }

    [Fact]
    public async Task AddItemToCart_UnavailableItem_ReturnsNull()
    {
        // Arrange
        await using var context = CreateContext();

        var cart = new Cart
        {
            CartID = 1,
            CustomerID = 1,
            CreatedDate = DateTime.UtcNow,
            TotalAmount = 0
        };

        var menuItem = new MenuItem
        {
            MenuItemID = 1,
            ItemName = "Unavailable Burger",
            Description = "Not available",
            Price = 50,
            Category = "Burgers",
            Availability = false
        };

        context.Carts.Add(cart);
        context.MenuItems.Add(menuItem);

        await context.SaveChangesAsync();

        var service = new CartService(context);

        var request = new AddCartItemRequest
        {
            MenuItemID = 1,
            Quantity = 1
        };

        // Act
        var result = await service.AddItemToCart(1, request);

        // Assert
        Assert.Null(result);
    }
}
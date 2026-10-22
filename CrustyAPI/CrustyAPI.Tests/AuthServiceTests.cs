using CrustyAPI.Data;
using CrustyAPI.DTOs;
using CrustyAPI.Models;
using CrustyAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace CrustyAPI.Tests;

public class AuthServiceTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CustomerLogin_ValidCredentials_ReturnsSuccess()
    {
        // Arrange
        await using var context = CreateContext();

        var password = "TestPassword123";

        context.Customers.Add(new Customer
        {
            CustomerID = 1,
            FirstName = "Test",
            LastName = "Customer",
            PhoneNumber = "0123456789",
            EmailAddress = "test@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        });

        await context.SaveChangesAsync();

        var service = new AuthService(context);

        var request = new LoginRequest
        {
            Identifier = "test@test.com",
            Password = password
        };

        // Act
        var result = await service.CustomerLogin(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(1, result.UserId);
        Assert.Equal("Customer", result.Role);
        Assert.Equal("test@test.com", result.Username);
        Assert.Equal("Login successful", result.Message);
    }

    [Fact]
    public async Task CustomerLogin_WrongPassword_ReturnsFailure()
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
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123")
        });

        await context.SaveChangesAsync();

        var service = new AuthService(context);

        var request = new LoginRequest
        {
            Identifier = "test@test.com",
            Password = "WrongPassword123"
        };

        // Act
        var result = await service.CustomerLogin(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Invalid email or password", result.Message);
    }

    [Fact]
    public async Task CustomerLogin_UnknownEmail_ReturnsFailure()
    {
        // Arrange
        await using var context = CreateContext();

        var service = new AuthService(context);

        var request = new LoginRequest
        {
            Identifier = "doesnotexist@test.com",
            Password = "Password123"
        };

        // Act
        var result = await service.CustomerLogin(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Invalid email or password", result.Message);
    }
}
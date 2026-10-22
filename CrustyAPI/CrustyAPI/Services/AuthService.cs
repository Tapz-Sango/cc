using CrustyAPI.Data;
using CrustyAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CrustyAPI.Services;

public class AuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<LoginResponse> CustomerLogin(LoginRequest request)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c =>
                c.EmailAddress.ToLower() == request.Identifier.ToLower());

        if (customer == null ||
            !BCrypt.Net.BCrypt.Verify(request.Password, customer.PasswordHash))
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Invalid email or password"
            };
        }

        return new LoginResponse
        {
            Success = true,
            UserId = customer.CustomerID,
            Username = customer.EmailAddress,
            Role = "Customer",
            Message = "Login successful"
        };
    }

    public async Task<LoginResponse> AdminLogin(LoginRequest request)
    {
        var admin = await _context.AdminUsers
            .FirstOrDefaultAsync(a =>
                a.Username.ToLower() == request.Identifier.ToLower());

        if (admin == null ||
            !BCrypt.Net.BCrypt.Verify(request.Password, admin.PasswordHash))
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Invalid username or password"
            };
        }

        return new LoginResponse
        {
            Success = true,
            UserId = admin.AdminID,
            Username = admin.Username,
            Role = "Admin",
            Message = "Login successful"
        };
    }
}
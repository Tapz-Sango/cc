using CrustyAPI.Data;
using CrustyAPI.DTOs;
using CrustyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CrustyAPI.Services;

public class CustomerService
{
    private readonly AppDbContext _context;

    public CustomerService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CustomerResponse>> GetAllCustomers()
    {
        return await _context.Customers
            .Select(c => new CustomerResponse
            {
                CustomerID = c.CustomerID,
                FirstName = c.FirstName,
                LastName = c.LastName,
                PhoneNumber = c.PhoneNumber,
                EmailAddress = c.EmailAddress,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<CustomerResponse?> GetCustomerById(int id)
    {
        return await _context.Customers
            .Where(c => c.CustomerID == id)
            .Select(c => new CustomerResponse
            {
                CustomerID = c.CustomerID,
                FirstName = c.FirstName,
                LastName = c.LastName,
                PhoneNumber = c.PhoneNumber,
                EmailAddress = c.EmailAddress,
                CreatedAt = c.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CustomerResponse?> CreateCustomer(CreateCustomerRequest request)
    {
        var emailExists = await _context.Customers
            .AnyAsync(c => c.EmailAddress.ToLower() == request.EmailAddress.ToLower());

        if (emailExists)
        {
            return null;
        }

        var customer = new Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            EmailAddress = request.EmailAddress,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return new CustomerResponse
        {
            CustomerID = customer.CustomerID,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PhoneNumber = customer.PhoneNumber,
            EmailAddress = customer.EmailAddress,
            CreatedAt = customer.CreatedAt
        };
    }

    public async Task<CustomerResponse?> UpdateCustomer(
        int id,
        UpdateCustomerRequest request)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.CustomerID == id);

        if (customer == null)
        {
            return null;
        }

        var emailExists = await _context.Customers
            .AnyAsync(c =>
                c.CustomerID != id &&
                c.EmailAddress.ToLower() == request.EmailAddress.ToLower());

        if (emailExists)
        {
            return null;
        }

        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.PhoneNumber = request.PhoneNumber;
        customer.EmailAddress = request.EmailAddress;

        await _context.SaveChangesAsync();

        return new CustomerResponse
        {
            CustomerID = customer.CustomerID,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PhoneNumber = customer.PhoneNumber,
            EmailAddress = customer.EmailAddress,
            CreatedAt = customer.CreatedAt
        };
    }

    public async Task<bool> DeleteCustomer(int id)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.CustomerID == id);

        if (customer == null)
        {
            return false;
        }

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return true;
    }
}
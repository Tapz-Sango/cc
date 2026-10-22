using CrustyAPI.Data;
using CrustyAPI.DTOs;
using CrustyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CrustyAPI.Services;

public class DeliveryAddressService
{
    private readonly AppDbContext _context;

    public DeliveryAddressService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DeliveryAddressResponse>> GetAllAddresses()
    {
        return await _context.DeliveryAddresses
            .Select(a => new DeliveryAddressResponse
            {
                DeliveryAddressID = a.DeliveryAddressID,
                Suburb = a.Suburb,
                HouseNumber = a.HouseNumber,
                StreetName = a.StreetName,
                BuildingType = a.BuildingType,
                CustomerID = a.CustomerID
            })
            .ToListAsync();
    }

    public async Task<DeliveryAddressResponse?> GetAddressById(int id)
    {
        return await _context.DeliveryAddresses
            .Where(a => a.DeliveryAddressID == id)
            .Select(a => new DeliveryAddressResponse
            {
                DeliveryAddressID = a.DeliveryAddressID,
                Suburb = a.Suburb,
                HouseNumber = a.HouseNumber,
                StreetName = a.StreetName,
                BuildingType = a.BuildingType,
                CustomerID = a.CustomerID
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<DeliveryAddressResponse>> GetAddressesByCustomer(int customerId)
    {
        return await _context.DeliveryAddresses
            .Where(a => a.CustomerID == customerId)
            .Select(a => new DeliveryAddressResponse
            {
                DeliveryAddressID = a.DeliveryAddressID,
                Suburb = a.Suburb,
                HouseNumber = a.HouseNumber,
                StreetName = a.StreetName,
                BuildingType = a.BuildingType,
                CustomerID = a.CustomerID
            })
            .ToListAsync();
    }

    public async Task<DeliveryAddressResponse?> CreateAddress(
        CreateDeliveryAddressRequest request)
    {
        var customerExists = await _context.Customers
            .AnyAsync(c => c.CustomerID == request.CustomerID);

        if (!customerExists)
        {
            return null;
        }

        var address = new DeliveryAddress
        {
            Suburb = request.Suburb,
            HouseNumber = request.HouseNumber,
            StreetName = request.StreetName,
            BuildingType = request.BuildingType,
            CustomerID = request.CustomerID
        };

        _context.DeliveryAddresses.Add(address);

        await _context.SaveChangesAsync();

        return new DeliveryAddressResponse
        {
            DeliveryAddressID = address.DeliveryAddressID,
            Suburb = address.Suburb,
            HouseNumber = address.HouseNumber,
            StreetName = address.StreetName,
            BuildingType = address.BuildingType,
            CustomerID = address.CustomerID
        };
    }

    public async Task<DeliveryAddressResponse?> UpdateAddress(
        int id,
        UpdateDeliveryAddressRequest request)
    {
        var address = await _context.DeliveryAddresses
            .FirstOrDefaultAsync(a => a.DeliveryAddressID == id);

        if (address == null)
        {
            return null;
        }

        address.Suburb = request.Suburb;
        address.HouseNumber = request.HouseNumber;
        address.StreetName = request.StreetName;
        address.BuildingType = request.BuildingType;

        await _context.SaveChangesAsync();

        return new DeliveryAddressResponse
        {
            DeliveryAddressID = address.DeliveryAddressID,
            Suburb = address.Suburb,
            HouseNumber = address.HouseNumber,
            StreetName = address.StreetName,
            BuildingType = address.BuildingType,
            CustomerID = address.CustomerID
        };
    }

    public async Task<bool> DeleteAddress(int id)
    {
        var address = await _context.DeliveryAddresses
            .FirstOrDefaultAsync(a => a.DeliveryAddressID == id);

        if (address == null)
        {
            return false;
        }

        _context.DeliveryAddresses.Remove(address);

        await _context.SaveChangesAsync();

        return true;
    }
}
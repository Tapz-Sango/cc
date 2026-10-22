using BCrypt.Net;
using CrustyAPI.Data;
using CrustyAPI.DTOs;
using CrustyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CrustyAPI.Services;

public class AdminStaffService
{
    private readonly AppDbContext _context;

    public AdminStaffService(AppDbContext context)
    {
        _context = context;
    }

    // =========================
    // STAFF
    // =========================

    public async Task<List<StaffResponse>> GetAllStaff()
    {
        return await _context.Staff
            .Select(s => new StaffResponse
            {
                StaffId = s.StaffId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Role = s.Role,
                PhoneNumber = s.PhoneNumber
            })
            .ToListAsync();
    }

    public async Task<StaffResponse?> GetStaffById(int id)
    {
        return await _context.Staff
            .Where(s => s.StaffId == id)
            .Select(s => new StaffResponse
            {
                StaffId = s.StaffId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Role = s.Role,
                PhoneNumber = s.PhoneNumber
            })
            .FirstOrDefaultAsync();
    }

    public async Task<StaffResponse> CreateStaff(CreateStaffRequest request)
    {
        var staff = new Staff
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = request.Role,
            PhoneNumber = request.PhoneNumber
        };

        _context.Staff.Add(staff);
        await _context.SaveChangesAsync();

        return new StaffResponse
        {
            StaffId = staff.StaffId,
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            Role = staff.Role,
            PhoneNumber = staff.PhoneNumber
        };
    }

    public async Task<StaffResponse?> UpdateStaff(
        int id,
        UpdateStaffRequest request)
    {
        var staff = await _context.Staff
            .FirstOrDefaultAsync(s => s.StaffId == id);

        if (staff == null)
        {
            return null;
        }

        staff.FirstName = request.FirstName;
        staff.LastName = request.LastName;
        staff.Role = request.Role;
        staff.PhoneNumber = request.PhoneNumber;

        await _context.SaveChangesAsync();

        return new StaffResponse
        {
            StaffId = staff.StaffId,
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            Role = staff.Role,
            PhoneNumber = staff.PhoneNumber
        };
    }

    public async Task<bool> DeleteStaff(int id)
    {
        var staff = await _context.Staff
            .FirstOrDefaultAsync(s => s.StaffId == id);

        if (staff == null)
        {
            return false;
        }

        _context.Staff.Remove(staff);
        await _context.SaveChangesAsync();

        return true;
    }

    // =========================
    // ADMIN USERS
    // =========================

    public async Task<List<AdminResponse>> GetAllAdmins()
    {
        return await _context.AdminUsers
            .Select(a => new AdminResponse
            {
                AdminID = a.AdminID,
                Username = a.Username,
                EmailAddress = a.EmailAddress,
                StaffId = a.StaffId
            })
            .ToListAsync();
    }

    public async Task<AdminResponse?> GetAdminById(int id)
    {
        return await _context.AdminUsers
            .Where(a => a.AdminID == id)
            .Select(a => new AdminResponse
            {
                AdminID = a.AdminID,
                Username = a.Username,
                EmailAddress = a.EmailAddress,
                StaffId = a.StaffId
            })
            .FirstOrDefaultAsync();
    }

    public async Task<AdminResponse?> CreateAdmin(
        CreateAdminRequest request)
    {
        var usernameExists = await _context.AdminUsers
            .AnyAsync(a => a.Username == request.Username);

        if (usernameExists)
        {
            return null;
        }

        var emailExists = await _context.AdminUsers
            .AnyAsync(a => a.EmailAddress == request.EmailAddress);

        if (emailExists)
        {
            return null;
        }

        if (request.StaffId.HasValue)
        {
            var staffExists = await _context.Staff
                .AnyAsync(s => s.StaffId == request.StaffId.Value);

            if (!staffExists)
            {
                return null;
            }
        }

        var admin = new AdminUser
        {
            Username = request.Username,
            EmailAddress = request.EmailAddress,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            StaffId = request.StaffId
        };

        _context.AdminUsers.Add(admin);
        await _context.SaveChangesAsync();

        return new AdminResponse
        {
            AdminID = admin.AdminID,
            Username = admin.Username,
            EmailAddress = admin.EmailAddress,
            StaffId = admin.StaffId
        };
    }

    public async Task<AdminResponse?> UpdateAdmin(
        int id,
        UpdateAdminRequest request)
    {
        var admin = await _context.AdminUsers
            .FirstOrDefaultAsync(a => a.AdminID == id);

        if (admin == null)
        {
            return null;
        }

        var usernameExists = await _context.AdminUsers
            .AnyAsync(a =>
                a.Username == request.Username &&
                a.AdminID != id);

        if (usernameExists)
        {
            return null;
        }

        var emailExists = await _context.AdminUsers
            .AnyAsync(a =>
                a.EmailAddress == request.EmailAddress &&
                a.AdminID != id);

        if (emailExists)
        {
            return null;
        }

        if (request.StaffId.HasValue)
        {
            var staffExists = await _context.Staff
                .AnyAsync(s => s.StaffId == request.StaffId.Value);

            if (!staffExists)
            {
                return null;
            }
        }

        admin.Username = request.Username;
        admin.EmailAddress = request.EmailAddress;
        admin.StaffId = request.StaffId;

        await _context.SaveChangesAsync();

        return new AdminResponse
        {
            AdminID = admin.AdminID,
            Username = admin.Username,
            EmailAddress = admin.EmailAddress,
            StaffId = admin.StaffId
        };
    }

    public async Task<bool> DeleteAdmin(int id)
    {
        var admin = await _context.AdminUsers
            .FirstOrDefaultAsync(a => a.AdminID == id);

        if (admin == null)
        {
            return false;
        }

        _context.AdminUsers.Remove(admin);
        await _context.SaveChangesAsync();

        return true;
    }
}
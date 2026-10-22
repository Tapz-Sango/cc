using CrustyAPI.DTOs;
using CrustyAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrustyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminStaffController : ControllerBase
{
    private readonly AdminStaffService _service;

    public AdminStaffController(AdminStaffService service)
    {
        _service = service;
    }

    // =========================
    // STAFF
    // =========================

    [HttpGet("staff")]
    public async Task<IActionResult> GetAllStaff()
    {
        var staff = await _service.GetAllStaff();

        return Ok(staff);
    }

    [HttpGet("staff/{id}")]
    public async Task<IActionResult> GetStaff(int id)
    {
        var staff = await _service.GetStaffById(id);

        if (staff == null)
        {
            return NotFound(new { message = "Staff member not found" });
        }

        return Ok(staff);
    }

    [HttpPost("staff")]
    public async Task<IActionResult> CreateStaff(
        CreateStaffRequest request)
    {
        var staff = await _service.CreateStaff(request);

        return CreatedAtAction(
            nameof(GetStaff),
            new { id = staff.StaffId },
            staff
        );
    }

    [HttpPut("staff/{id}")]
    public async Task<IActionResult> UpdateStaff(
        int id,
        UpdateStaffRequest request)
    {
        var staff = await _service.UpdateStaff(id, request);

        if (staff == null)
        {
            return NotFound(new { message = "Staff member not found" });
        }

        return Ok(staff);
    }

    [HttpDelete("staff/{id}")]
    public async Task<IActionResult> DeleteStaff(int id)
    {
        var deleted = await _service.DeleteStaff(id);

        if (!deleted)
        {
            return NotFound(new { message = "Staff member not found" });
        }

        return NoContent();
    }

    // =========================
    // ADMIN USERS
    // =========================

    [HttpGet("admins")]
    public async Task<IActionResult> GetAllAdmins()
    {
        var admins = await _service.GetAllAdmins();

        return Ok(admins);
    }

    [HttpGet("admins/{id}")]
    public async Task<IActionResult> GetAdmin(int id)
    {
        var admin = await _service.GetAdminById(id);

        if (admin == null)
        {
            return NotFound(new { message = "Admin user not found" });
        }

        return Ok(admin);
    }

    [HttpPost("admins")]
    public async Task<IActionResult> CreateAdmin(
        CreateAdminRequest request)
    {
        var admin = await _service.CreateAdmin(request);

        if (admin == null)
        {
            return BadRequest(new
            {
                message = "Username, email, or Staff ID is invalid or already exists."
            });
        }

        return CreatedAtAction(
            nameof(GetAdmin),
            new { id = admin.AdminID },
            admin
        );
    }

    [HttpPut("admins/{id}")]
    public async Task<IActionResult> UpdateAdmin(
        int id,
        UpdateAdminRequest request)
    {
        var admin = await _service.UpdateAdmin(id, request);

        if (admin == null)
        {
            return BadRequest(new
            {
                message = "Admin not found, or username, email, or Staff ID is invalid."
            });
        }

        return Ok(admin);
    }

    [HttpDelete("admins/{id}")]
    public async Task<IActionResult> DeleteAdmin(int id)
    {
        var deleted = await _service.DeleteAdmin(id);

        if (!deleted)
        {
            return NotFound(new { message = "Admin user not found" });
        }

        return NoContent();
    }
}
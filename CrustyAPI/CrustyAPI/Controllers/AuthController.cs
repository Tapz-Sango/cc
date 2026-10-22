using CrustyAPI.DTOs;
using CrustyAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrustyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("customer/login")]
    public async Task<IActionResult> CustomerLogin(LoginRequest request)
    {
        var response = await _authService.CustomerLogin(request);

        if (!response.Success)
        {
            return Unauthorized(response);
        }

        return Ok(response);
    }

    [HttpPost("admin/login")]
    public async Task<IActionResult> AdminLogin(LoginRequest request)
    {
        var response = await _authService.AdminLogin(request);

        if (!response.Success)
        {
            return Unauthorized(response);
        }

        return Ok(response);
    }
}
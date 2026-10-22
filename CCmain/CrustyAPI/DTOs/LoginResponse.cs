namespace CrustyAPI.DTOs;

public class LoginResponse
{
    public bool Success { get; set; }

    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}
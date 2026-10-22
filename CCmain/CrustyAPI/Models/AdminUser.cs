namespace CrustyAPI.Models;

public class AdminUser
{
    public int AdminID { get; set; }

    public string Username { get; set; } = string.Empty;

    public string EmailAddress { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public int? StaffId { get; set; }
}
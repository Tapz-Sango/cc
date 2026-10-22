namespace CrustyAPI.Models;

public class Staff
{
    public int StaffId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;
}
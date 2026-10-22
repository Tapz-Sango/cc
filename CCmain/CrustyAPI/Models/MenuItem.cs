namespace CrustyAPI.Models;

public class MenuItem
{
    public int MenuItemID { get; set; }

    public string ItemName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string Category { get; set; } = string.Empty;

    public bool Availability { get; set; }

    public string? ImageUrl { get; set; }
    public bool IsPopular { get; set; } = false;
}
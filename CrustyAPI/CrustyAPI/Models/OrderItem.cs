namespace CrustyAPI.Models;

public class OrderItem
{
    public int OrderItemID { get; set; }

    public int Quantity { get; set; }

    public decimal ItemPrice { get; set; }

    public string? Notes { get; set; }

    public int OrderID { get; set; }

    public int MenuItemID { get; set; }
}
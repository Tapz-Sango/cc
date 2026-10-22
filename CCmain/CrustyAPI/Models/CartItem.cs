namespace CrustyAPI.Models;

public class CartItem
{
    public int CartItemID { get; set; }

    public decimal ItemPrice { get; set; }

    public int Quantity { get; set; }

    public int MenuItemID { get; set; }

    public int CartID { get; set; }
}
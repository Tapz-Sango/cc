namespace CrustyAPI.Models;

public class Cart
{
    public int CartID { get; set; }

    public DateTime CreatedDate { get; set; }

    public decimal TotalAmount { get; set; }

    public int CustomerID { get; set; }
}
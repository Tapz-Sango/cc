namespace CrustyAPI.Models;

public class Order
{
    public int OrderID { get; set; }

    public DateTime OrderDate { get; set; }

    public string OrderStatus { get; set; } = string.Empty;

    public string DeliveryMethod { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public int? DeliveryAddressID { get; set; }

    public int CustomerID { get; set; }

    public int? StaffId { get; set; }
}
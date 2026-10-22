namespace CrustyAPI.Models;

public class Payment
{
    public int PaymentID { get; set; }

    public DateTime PaymentDate { get; set; }

    public decimal Amount { get; set; }

    public string PaymentStatus { get; set; } = string.Empty;

    public int PaymentMethodID { get; set; }

    public int OrderID { get; set; }
}
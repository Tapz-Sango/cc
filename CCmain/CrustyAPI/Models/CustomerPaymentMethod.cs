namespace CrustyAPI.Models;

public class CustomerPaymentMethod
{
    public int CustomerPaymentMethodID { get; set; }

    public int CustomerID { get; set; }

    public int PaymentMethodID { get; set; }

    public string? CardholderName { get; set; }

    public string? CardLast4 { get; set; }

    public int? ExpiryMonth { get; set; }

    public int? ExpiryYear { get; set; }

    public Customer Customer { get; set; } = null!;

    public PaymentMethod PaymentMethod { get; set; } = null!;
}
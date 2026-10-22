
namespace CrustyCrab.Models;

public class ApiCustomerReview
{
    public int ReviewID { get; set; }

    public int CustomerID { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public string? AdminResponse { get; set; }

    public int? AdminID { get; set; }

    public string? AdminUsername { get; set; }

    public DateTime? RespondedAt { get; set; }
}

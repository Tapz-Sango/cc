
namespace CrustyAPI.Models;

public class CustomerReview
{
    public int ReviewID { get; set; }

    public int CustomerID { get; set; }

    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public string? AdminResponse { get; set; }

    public int? AdminID { get; set; }

    public DateTime? RespondedAt { get; set; }
}

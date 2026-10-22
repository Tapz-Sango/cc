
using System.ComponentModel.DataAnnotations;

namespace CrustyAPI.DTOs;

public class CreateReviewRequest
{
    [Range(1, 5)]
    public int Rating { get; set; }

    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Comment { get; set; } = string.Empty;
}

public class AdminReviewResponseRequest
{
    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string AdminResponse { get; set; } = string.Empty;
}

public class CustomerReviewResponse
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

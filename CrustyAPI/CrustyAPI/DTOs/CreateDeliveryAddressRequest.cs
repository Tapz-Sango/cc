using System.ComponentModel.DataAnnotations;

namespace CrustyAPI.DTOs;

public class CreateDeliveryAddressRequest
{
    [Required]
    public string Suburb { get; set; } = string.Empty;

    [Required]
    public string HouseNumber { get; set; } = string.Empty;

    [Required]
    public string StreetName { get; set; } = string.Empty;

    public string? BuildingType { get; set; }

    [Required]
    public int CustomerID { get; set; }
}
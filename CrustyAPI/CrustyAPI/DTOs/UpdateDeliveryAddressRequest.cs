using System.ComponentModel.DataAnnotations;

namespace CrustyAPI.DTOs;

public class UpdateDeliveryAddressRequest
{
    [Required]
    public string Suburb { get; set; } = string.Empty;

    [Required]
    public string HouseNumber { get; set; } = string.Empty;

    [Required]
    public string StreetName { get; set; } = string.Empty;

    public string? BuildingType { get; set; }
}
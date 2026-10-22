namespace CrustyAPI.Models;

public class DeliveryAddress
{
    public int DeliveryAddressID { get; set; }

    public string Suburb { get; set; } = string.Empty;

    public string HouseNumber { get; set; } = string.Empty;

    public string StreetName { get; set; } = string.Empty;

    public string? BuildingType { get; set; }

    public int CustomerID { get; set; }
}
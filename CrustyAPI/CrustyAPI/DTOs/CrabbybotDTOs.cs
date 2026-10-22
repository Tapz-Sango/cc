namespace CrustyAPI.DTOs;

public class CrabbybotOptionResponse
{
    public string Id { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}

public class CrabbybotRequest
{
    public string OptionId { get; set; } = string.Empty;
}

public class CrabbybotResponse
{
    public string Message { get; set; } = string.Empty;
    public List<CrabbybotOptionResponse> Options { get; set; } = new();
}

namespace CrustyCrab.Models
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string SpiceLevel { get; set; } = "Medium (Default)";
        public List<string> Addons { get; set; } = new();
        public decimal TotalPrice { get; set; }
        public string FulfillmentMethod { get; set; } = "Collection";
        public string Status { get; set; } = "Pending";
        public string TimeOrdered { get; set; } = string.Empty;
    }
}
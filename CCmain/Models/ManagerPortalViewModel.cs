
namespace CrustyCrab.Models
{
    public class ManagerPortalViewModel
    {
        public List<InventoryItem> InventoryItems { get; set; } = new();
        public List<ApiMenuItem> MenuItems { get; set; } = new();
        public decimal TodaysRevenue { get; set; } = 14850.00m;
        public string PeakOrderTime { get; set; } = "18:00 - 20:00";
        public string StaffRecommendation { get; set; } = "Based on analytics, increase kitchen staffing by 2 team members between 17:30 and 21:00 on weekends to handle surge volume.";
    }

    public class InventoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = "In Stock";
    }
}
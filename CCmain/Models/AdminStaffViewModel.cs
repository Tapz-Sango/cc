namespace CrustyCrab.Models
{
    public class AdminStaffViewModel
    {
        public List<ApiStaff> Staff { get; set; } = new();
        public List<ApiAdmin> Admins { get; set; } = new();
        public List<ApiCustomer> Customers { get; set; } = new();
    }
}

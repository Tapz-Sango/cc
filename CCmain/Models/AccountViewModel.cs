namespace CrustyCrab.Models
{
    public class AccountViewModel
    {
        public ApiCustomer Customer { get; set; } = new();
        public List<ApiDeliveryAddress> Addresses { get; set; } = new();
        public List<ApiOrder> Orders { get; set; } = new();
        public List<ApiCustomerPaymentMethod> SavedPaymentMethods { get; set; } = new();
        public List<ApiPaymentMethod> PaymentMethods { get; set; } = new();
    }
}

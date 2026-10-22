namespace CrustyCrab.Models
{
    public class CheckoutViewModel
    {
        public int MenuItemId { get; set; }
        public bool FromCart { get; set; }
        public ApiCart? Cart { get; set; }
        public string SpiceLevel { get; set; } = "Medium";
        public List<string> Addons { get; set; } = new();
        public string FulfillmentMethod { get; set; } = "Collection";
        public int? DeliveryAddressId { get; set; }
        public int PaymentMethodId { get; set; }
        public string CardholderName { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public string ExpiryDate { get; set; } = string.Empty;
        public string Cvc { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public List<ApiDeliveryAddress> Addresses { get; set; } = new();
        public List<ApiPaymentMethod> PaymentMethods { get; set; } = new();
        public List<ApiCustomerPaymentMethod> SavedPaymentMethods { get; set; } = new();
        public decimal DeliveryFee => FulfillmentMethod == "Delivery" ? 50.00m : 0.00m;
        public decimal Total => Subtotal + DeliveryFee;
    }
}

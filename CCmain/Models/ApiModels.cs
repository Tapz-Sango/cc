namespace CrustyCrab.Models
{
    public class ApiMenuItem
    {
        public int MenuItemID { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool Availability { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsPopular { get; set; } = false;
    }

    public class ApiOrderItem
    {
        public int OrderItemID { get; set; }
        public int MenuItemID { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal ItemPrice { get; set; }
        public decimal Subtotal { get; set; }
        public string? Notes { get; set; }
    }

    public class ApiOrder
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public string DeliveryMethod { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int? DeliveryAddressID { get; set; }
        public int CustomerID { get; set; }
        public int? StaffId { get; set; }
        public List<ApiOrderItem> Items { get; set; } = new();
    }

    public class ApiCustomer
    {
        public int CustomerID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class ApiDeliveryAddress
    {
        public int DeliveryAddressID { get; set; }
        public string Suburb { get; set; } = string.Empty;
        public string HouseNumber { get; set; } = string.Empty;
        public string StreetName { get; set; } = string.Empty;
        public string? BuildingType { get; set; }
        public int CustomerID { get; set; }
    }

    public class ApiCartItem
    {
        public int CartItemID { get; set; }
        public int MenuItemID { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal ItemPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class ApiCart
    {
        public int CartID { get; set; }
        public int CustomerID { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<ApiCartItem> Items { get; set; } = new();
    }

    public class ApiPaymentMethod
    {
        public int PaymentMethodID { get; set; }
        public string MethodName { get; set; } = string.Empty;
    }

    public class ApiCustomerPaymentMethod
    {
        public int CustomerPaymentMethodID { get; set; }
        public int CustomerID { get; set; }
        public int PaymentMethodID { get; set; }
        public string MethodName { get; set; } = string.Empty;
        public string? CardholderName { get; set; }
        public string? CardLast4 { get; set; }
        public int? ExpiryMonth { get; set; }
        public int? ExpiryYear { get; set; }
    }

    public class ApiPayment
    {
        public int PaymentID { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public int PaymentMethodID { get; set; }
        public string MethodName { get; set; } = string.Empty;
        public int OrderID { get; set; }
    }

    public class ApiLoginResponse
    {
        public bool Success { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class ApiStaff
    {
        public int StaffId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class ApiAdmin
    {
        public int AdminID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public int? StaffId { get; set; }
    }

    public class ApiCrabbyBotOption
    {
        public string Id { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }

    public class ApiCrabbyBotResponse
    {
        public string Message { get; set; } = string.Empty;
        public List<ApiCrabbyBotOption> Options { get; set; } = new();
    }
}

namespace CrustyCrab.Models
{
    public class CustomerRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class CustomerUpdateRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Identifier { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class CartRequest
    {
        public int CustomerID { get; set; }
    }

    public class CartItemRequest
    {
        public int MenuItemID { get; set; }
        public int Quantity { get; set; }
    }

    public class CartItemUpdateRequest
    {
        public int Quantity { get; set; }
    }

    public class DeliveryAddressRequest
    {
        public string Suburb { get; set; } = string.Empty;
        public string HouseNumber { get; set; } = string.Empty;
        public string StreetName { get; set; } = string.Empty;
        public string? BuildingType { get; set; }
        public int CustomerID { get; set; }
    }

    public class DeliveryAddressUpdateRequest
    {
        public string Suburb { get; set; } = string.Empty;
        public string HouseNumber { get; set; } = string.Empty;
        public string StreetName { get; set; } = string.Empty;
        public string? BuildingType { get; set; }
    }

    public class PaymentMethodRequest
    {
        public string MethodName { get; set; } = string.Empty;
    }

    public class SavedPaymentMethodRequest
    {
        public int CustomerID { get; set; }
        public int PaymentMethodID { get; set; }
        public string CardholderName { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
    }

    public class PaymentRequest
    {
        public int OrderID { get; set; }
        public int PaymentMethodID { get; set; }
        public decimal Amount { get; set; }
    }

    public class PaymentStatusRequest
    {
        public string PaymentStatus { get; set; } = string.Empty;
    }

    public class StaffRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class AdminRequest
    {
        public string Username { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int? StaffId { get; set; }
    }

    public class AdminUpdateRequest
    {
        public string Username { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public int? StaffId { get; set; }
    }

    public class CrabbyBotRequest
    {
        public string OptionId { get; set; } = string.Empty;
    }
}

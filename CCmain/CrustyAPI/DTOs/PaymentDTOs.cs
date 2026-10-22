using System.ComponentModel.DataAnnotations;

namespace CrustyAPI.DTOs;

public class CreatePaymentMethodRequest
{
    [Required]
    public string MethodName { get; set; } = string.Empty;
}

public class CreatePaymentRequest
{
    [Required]
    public int OrderID { get; set; }

    [Required]
    public int PaymentMethodID { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }
}

public class UpdatePaymentStatusRequest
{
    [Required]
    public string PaymentStatus { get; set; } = string.Empty;
}

public class PaymentMethodResponse
{
    public int PaymentMethodID { get; set; }

    public string MethodName { get; set; } = string.Empty;
}

public class PaymentResponse
{
    public int PaymentID { get; set; }

    public DateTime PaymentDate { get; set; }

    public decimal Amount { get; set; }

    public string PaymentStatus { get; set; } = string.Empty;

    public int PaymentMethodID { get; set; }

    public string MethodName { get; set; } = string.Empty;

    public int OrderID { get; set; }
    public class CreateCustomerPaymentMethodRequest
    {
        [Required]
        public int CustomerID { get; set; }

        [Required]
        public int PaymentMethodID { get; set; }

        public string? CardholderName { get; set; }

        [StringLength(4, MinimumLength = 4)]
        public string? CardLast4 { get; set; }

        [Range(1, 12)]
        public int? ExpiryMonth { get; set; }

        [Range(2026, 2100)]
        public int? ExpiryYear { get; set; }
    }

    public class CustomerPaymentMethodResponse
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

    public class CreateSavedCardRequest
    {
        [Required]
        public int CustomerID { get; set; }

        [Required]
        public int PaymentMethodID { get; set; }

        [Required]
        public string CardholderName { get; set; } = string.Empty;

        [Required]
        [StringLength(16, MinimumLength = 13)]
        public string CardNumber { get; set; } = string.Empty;

        [Required]
        [Range(1, 12)]
        public int ExpiryMonth { get; set; }

        [Required]
        [Range(2026, 2100)]
        public int ExpiryYear { get; set; }
    }
}
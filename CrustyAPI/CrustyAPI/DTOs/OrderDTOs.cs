using System.ComponentModel.DataAnnotations;

namespace CrustyAPI.DTOs;

public class CreateOrderRequest
{
    [Required]
    public int CustomerID { get; set; }

    public int? DeliveryAddressID { get; set; }

    [Required]
    public string DeliveryMethod { get; set; } = string.Empty;

    [Required]
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}

public class CreateOrderItemRequest
{
    [Required]
    public int MenuItemID { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    public string? Notes { get; set; }
}

public class UpdateOrderStatusRequest
{
    [Required]
    public string OrderStatus { get; set; } = string.Empty;
}

public class OrderItemResponse
{
    public int OrderItemID { get; set; }

    public int MenuItemID { get; set; }

    public string ItemName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal ItemPrice { get; set; }

    public decimal Subtotal { get; set; }

    public string? Notes { get; set; }
}

public class OrderResponse
{
    public int OrderID { get; set; }

    public DateTime OrderDate { get; set; }

    public string OrderStatus { get; set; } = string.Empty;

    public string DeliveryMethod { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public int? DeliveryAddressID { get; set; }

    public int CustomerID { get; set; }

    public int? StaffId { get; set; }

    public List<OrderItemResponse> Items { get; set; } = new();
}
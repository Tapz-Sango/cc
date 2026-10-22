using System.ComponentModel.DataAnnotations;

namespace CrustyAPI.DTOs;

public class CreateCartRequest
{
    [Required]
    public int CustomerID { get; set; }
}

public class AddCartItemRequest
{
    [Required]
    public int MenuItemID { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}

public class UpdateCartItemRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}

public class CartItemResponse
{
    public int CartItemID { get; set; }

    public int MenuItemID { get; set; }

    public string ItemName { get; set; } = string.Empty;

    public decimal ItemPrice { get; set; }

    public int Quantity { get; set; }

    public decimal Subtotal { get; set; }
}

public class CartResponse
{
    public int CartID { get; set; }

    public int CustomerID { get; set; }

    public DateTime CreatedDate { get; set; }

    public decimal TotalAmount { get; set; }

    public List<CartItemResponse> Items { get; set; } = new();
}
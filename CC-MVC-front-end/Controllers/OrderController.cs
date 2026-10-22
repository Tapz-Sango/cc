using CrustyCrab.Models;
using CrustyCrab.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrustyCrab.Controllers
{
    public class OrderController : Controller
    {
        private readonly CrustyApiClient _api;

        public OrderController(CrustyApiClient api)
        {
            _api = api;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orders = await _api.GetOrders();
            return orders == null ? StatusCode(502) : Json(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var order = await _api.GetOrder(id);
            return order == null ? NotFound() : Json(order);
        }

        [HttpGet]
        public async Task<IActionResult> Customer(int customerId)
        {
            var orders = await _api.GetCustomerOrders(customerId);
            return orders == null ? StatusCode(502) : Json(orders);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            var order = await _api.CreateOrder(request.CustomerID, request.DeliveryAddressID, request.DeliveryMethod, request.Items);
            return order == null ? BadRequest() : Json(order);
        }

        [HttpPost]
        public async Task<IActionResult> CancelForm(int id, int customerId)
        {
            var sessionCustomerId = HttpContext.Session.GetInt32("UserId");
            if (!sessionCustomerId.HasValue || sessionCustomerId.Value != customerId)
                return RedirectToAction("Login", "Auth");

            var order = await _api.CancelOrder(id, customerId);
            TempData[order == null ? "Error" : "Success"] = order == null ? "The order could not be cancelled." : "The order has been cancelled.";
            return RedirectToAction("Index", "Customer");
        }

        [HttpPut]
        public async Task<IActionResult> Cancel(int id, int customerId)
        {
            var order = await _api.CancelOrder(id, customerId);
            return order == null ? BadRequest() : Json(order);
        }

        [HttpPut]
        public async Task<IActionResult> Status(int id, [FromBody] OrderStatusRequest request)
        {
            var order = await _api.UpdateOrderStatus(id, request.OrderStatus);
            return order == null ? NotFound() : Json(order);
        }
    }

    public class CreateOrderRequest
    {
        public int CustomerID { get; set; }
        public int? DeliveryAddressID { get; set; }
        public string DeliveryMethod { get; set; } = string.Empty;
        public List<ApiOrderItemRequest> Items { get; set; } = new();
    }

    public class OrderStatusRequest
    {
        public string OrderStatus { get; set; } = string.Empty;
    }
}

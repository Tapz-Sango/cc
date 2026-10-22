using Microsoft.AspNetCore.Mvc;
using CrustyCrab.Models;
using CrustyCrab.Services;

namespace CrustyCrab.Controllers
{
    public class KitchenController : Controller
    {
        private static readonly string[] ClosedStatuses = { "Ready", "Completed", "Delivered", "Cancelled" };

        private readonly CrustyApiClient _api;

        public KitchenController(CrustyApiClient api)
        {
            _api = api;
        }

        public async Task<IActionResult> Kds()
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue ||
    !string.Equals(
        HttpContext.Session.GetString("Role"),
        "Admin",
        StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Login", "Manager");
            }
            var orders = await _api.GetOrders();
            if (orders == null)
            {
                ViewData["ApiError"] = "Live orders are unavailable right now.";
            }

            var active = (orders ?? new List<ApiOrder>())
                .Where(o => !ClosedStatuses.Contains(o.OrderStatus, StringComparer.OrdinalIgnoreCase))
                .OrderBy(o => o.OrderDate)
                .Select(ToViewModel)
                .ToList();

            return View(active);
        }

        [HttpGet]
        public async Task<IActionResult> Tracker()
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue ||
                !string.Equals(
                    HttpContext.Session.GetString("Role"),
                    "Customer",
                    StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Login", "Auth");
            }

            var orders = await _api.GetOrders();

            if (orders == null)
            {
                ViewData["ApiError"] = "Your order status is currently unavailable.";
                return View(new List<ApiOrder>());
            }

            var userId = HttpContext.Session.GetInt32("UserId");

            var customerOrders = orders
                .Where(o => o.CustomerID == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(customerOrders);
        }
        [HttpPost]
        public async Task<IActionResult> MarkReady(int orderId)
        {
            await _api.UpdateOrderStatus(orderId, "Ready");
            return RedirectToAction(nameof(Kds));
        }

        private static OrderViewModel ToViewModel(ApiOrder order)
        {
            var first = order.Items.FirstOrDefault();
            var spice = "Medium Spice";
            var addons = new List<string>();

            foreach (var part in (first?.Notes ?? string.Empty).Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (part.StartsWith("Spice:", StringComparison.OrdinalIgnoreCase))
                {
                    var level = part.Substring(6).Trim();
                    spice = level.Contains("Hot", StringComparison.OrdinalIgnoreCase) || level.Contains("Mild", StringComparison.OrdinalIgnoreCase) ? level : level + " Spice";
                }
                else if (part.StartsWith("Add-ons:", StringComparison.OrdinalIgnoreCase))
                {
                    addons.AddRange(part.Substring(8).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
                }
            }

            var minutes = (int)Math.Max(0, (DateTime.UtcNow - DateTime.SpecifyKind(order.OrderDate, DateTimeKind.Utc)).TotalMinutes);

            return new OrderViewModel
            {
                OrderId = order.OrderID,
                CustomerName = "#CC-" + order.CustomerID,
                ItemName = first == null ? "Order" : $"{first.Quantity}x {first.ItemName}",
                SpiceLevel = spice,
                Addons = addons,
                TotalPrice = order.TotalAmount,
                FulfillmentMethod = order.DeliveryMethod,
                Status = order.OrderStatus,
                TimeOrdered = minutes + (minutes == 1 ? " min ago" : " mins ago")
            };
        }
    }
}

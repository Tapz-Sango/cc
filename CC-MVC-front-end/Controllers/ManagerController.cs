using Microsoft.AspNetCore.Mvc;
using CrustyCrab.Models;
using CrustyCrab.Services;

namespace CrustyCrab.Controllers
{
    public class ManagerController : Controller
    {
        private readonly CrustyApiClient _api;

        public ManagerController(CrustyApiClient api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("UserId").HasValue &&
                string.Equals(
                    HttpContext.Session.GetString("Role"),
                    "Admin",
                    StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction(nameof(Portal));
            }

            return View(new LoginRequest());
        }
        public async Task<IActionResult> Portal()
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue || !string.Equals(HttpContext.Session.GetString("Role"), "Admin", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction(nameof(Login));

            var model = new ManagerPortalViewModel();

            var menu = await _api.GetMenu();
            var orders = await _api.GetOrders();

            if (menu == null || orders == null)
            {
                ViewData["ApiError"] = "Some portal data is unavailable right now.";
            }

            model.MenuItems = (menu ?? new List<ApiMenuItem>())
     .Select(item =>
     {
         item.ImageUrl = _api.ResolveApiUrl(item.ImageUrl);
         return item;
     })
     .ToList();
            model.InventoryItems = (menu ?? new List<ApiMenuItem>())
                .Select(m => new InventoryItem
                {
                    Id = m.MenuItemID,
                    Name = m.ItemName,
                    Status = m.Availability ? "In Stock" : "Out of Stock"
                })
                .ToList();

            if (orders != null)
            {
                var today = DateTime.UtcNow.Date;
                var todays = orders
                    .Where(o => o.OrderDate.Date == today && !o.OrderStatus.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                model.TodaysRevenue = todays.Sum(o => o.TotalAmount);

                var peak = orders
                    .GroupBy(o => o.OrderDate.Hour)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault();

                if (peak != null)
                {
                    model.PeakOrderTime = $"{peak.Key:00}:00 - {(peak.Key + 2) % 24:00}:00";
                }
            }

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> MenuManagement()
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue ||
                !string.Equals(
                    HttpContext.Session.GetString("Role"),
                    "Admin",
                    StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction(nameof(Login));
            }

            var menu = await _api.GetMenu();

            var items = (menu ?? new List<ApiMenuItem>())
                .Select(item =>
                {
                    item.ImageUrl = _api.ResolveApiUrl(item.ImageUrl);
                    return item;
                })
                .ToList();

            return View(items);
        }

        [HttpGet]
        public IActionResult AddMenuItem()
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue ||
                !string.Equals(
                    HttpContext.Session.GetString("Role"),
                    "Admin",
                    StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction(nameof(Login));
            }

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ToggleStock(int id)
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue || !string.Equals(HttpContext.Session.GetString("Role"), "Admin", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Login", "Auth");

            var item = await _api.GetMenuItem(id);
            if (item != null)
            {
                item.Availability = !item.Availability;
                await _api.UpdateMenuItem(item);
            }
            return RedirectToAction(nameof(Portal));
        }
    }
}

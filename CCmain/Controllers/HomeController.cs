
using CrustyCrab.Models;
using CrustyCrab.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CrustyCrab.Controllers
{
    public class HomeController : Controller
    {
        private readonly CrustyApiClient _api;

        public HomeController(CrustyApiClient api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index()
        {
            var apiItems = await _api.GetPopularMenuItems();

            var items = (apiItems ?? new List<ApiMenuItem>())
                .Where(item => item.IsPopular && item.Availability)
                .Select(item => new MenuItem
                {
                    Id = item.MenuItemID,
                    Name = item.ItemName,
                    Category = item.Category,
                    Description = item.Description ?? string.Empty,
                    Price = item.Price,
                    IsInStock = item.Availability,
                    ImageUrl = _api.ResolveApiUrl(item.ImageUrl)
                })
                .ToList();

            return View(items);
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }
    }
}

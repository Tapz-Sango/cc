using Microsoft.AspNetCore.Mvc;
using CrustyCrab.Models;
using CrustyCrab.Services;

namespace CrustyCrab.Controllers
{
    public class MenuController : Controller
    {
        private readonly CrustyApiClient _api;

        public MenuController(CrustyApiClient api)
        {
            _api = api;
        }

        internal MenuItem ToMenuItem(ApiMenuItem item)
        {
            return new MenuItem
            {
                Id = item.MenuItemID,
                Name = item.ItemName,
                Category = item.Category,
                Description = item.Description ?? string.Empty,
                Price = item.Price,
                IsInStock = item.Availability,
                ImageUrl = _api.ResolveApiUrl(item.ImageUrl)
            };
        }

        public async Task<IActionResult> Index(string category = "All Items")
        {
            var apiItems = await _api.GetMenu();
            if (apiItems == null)
                ViewData["ApiError"] = "The menu is unavailable right now. Please try again shortly.";

            var items = (apiItems ?? new List<ApiMenuItem>()).Select(ToMenuItem).ToList();
            if (category != "All Items")
                items = items.Where(m => m.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();

            ViewData["CurrentCategory"] = category;
            return View(items);
        }

        
        public async Task<IActionResult> Customise(int id)
        {
            var apiItem = await _api.GetMenuItem(id);
            if (apiItem == null)
            {
                var menu = await _api.GetMenu();
                apiItem = menu?.FirstOrDefault(m => m.Availability) ?? menu?.FirstOrDefault();
            }
            if (apiItem == null)
            {
                TempData["Error"] = "That item could not be loaded.";
                return RedirectToAction(nameof(Index));
            }
            return View(ToMenuItem(apiItem));
        }

        [HttpPost]
        public async Task<IActionResult> CreateForm(
     ApiMenuItem item,
     IFormFile? image)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            // Create the menu item first
            var created = await _api.CreateMenuItem(item);

            if (created == null)
            {
                TempData["Error"] = "The menu item could not be created.";
                return RedirectToAction("Portal", "Manager");
            }

            // Upload the image using the newly created MenuItemID
            if (image != null && image.Length > 0)
            {
                var uploaded = await _api.UploadMenuImage(
                    created.MenuItemID,
                    image);

                if (uploaded == null)
                {
                    TempData["Error"] =
                        "Menu item was created, but the image could not be uploaded.";

                    return RedirectToAction("Portal", "Manager");
                }
            }

            TempData["Success"] = "Menu item created successfully.";

            return RedirectToAction("Portal", "Manager");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            var item = await _api.GetMenuItem(id);

            if (item == null)
            {
                TempData["Error"] = "The menu item could not be loaded.";
                return RedirectToAction("Portal", "Manager");
            }

            return View(item);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateForm(int id, ApiMenuItem item)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            item.MenuItemID = id;
            var updated = await _api.UpdateMenuItem(item);
            TempData[updated == null ? "Error" : "Success"] = updated == null ? "The menu item could not be updated." : "Menu item updated.";
            return RedirectToAction("Portal", "Manager");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteForm(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            var result = await _api.DeleteMenuItem(id);

            if (result.Success)
            {
                TempData["Success"] = "Menu item deleted.";
            }
            else
            {
                TempData["Error"] = result.ErrorMessage;
            }

            return RedirectToAction("Portal", "Manager");
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ApiMenuItem item)
        {
            var created = await _api.CreateMenuItem(item);
            return created == null ? BadRequest() : Json(created);
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] ApiMenuItem item)
        {
            item.MenuItemID = id;
            var updated = await _api.UpdateMenuItem(item);
            return updated == null ? NotFound() : Json(updated);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _api.DeleteMenuItem(id);

            if (!result.Success)
                return NotFound(new { message = result.ErrorMessage });

            return NoContent();
        }

        

        private bool IsAdmin()
        {
            return HttpContext.Session.GetInt32("UserId").HasValue &&
                   string.Equals(HttpContext.Session.GetString("Role"), "Admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
using CrustyCrab.Models;
using CrustyCrab.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrustyCrab.Controllers
{
    public class CartController : Controller
    {
        private readonly CrustyApiClient _api;

        public CartController(CrustyApiClient api)
        {
            _api = api;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var customerId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");
            if (!customerId.HasValue || !string.Equals(role, "Customer", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Index), "Cart") });

            var cart = await GetOrCreateCart(customerId.Value);
            if (cart == null)
            {
                TempData["Error"] = "Your cart could not be loaded.";
                return View(new ApiCart());
            }

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddFromMenu(int menuItemId, int quantity = 1)
        {
            var customerId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");
            if (!customerId.HasValue || !string.Equals(role, "Customer", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Customise", "Menu", new { id = menuItemId }) });

            var cart = await GetOrCreateCart(customerId.Value);
            if (cart == null)
            {
                TempData["Error"] = "Your cart could not be created.";
                return RedirectToAction("Customise", "Menu", new { id = menuItemId });
            }

            var updated = await _api.AddCartItem(cart.CartID, menuItemId, Math.Max(1, quantity));
            TempData[updated == null ? "Error" : "Success"] = updated == null
                ? "The item could not be added to your cart."
                : "Item added to your cart.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateItem(int cartItemId, int quantity)
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction("Login", "Auth");

            await _api.UpdateCartItem(cartItemId, Math.Max(1, quantity));
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction("Login", "Auth");

            await _api.RemoveCartItem(cartItemId);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Customer(int customerId)
        {
            var cart = await _api.GetCustomerCart(customerId);
            return cart == null ? NotFound() : Json(cart);
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var cart = await _api.GetCart(id);
            return cart == null ? NotFound() : Json(cart);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CartRequest request)
        {
            var cart = await _api.CreateCart(request.CustomerID);
            return cart == null ? BadRequest() : Json(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(int cartId, [FromBody] CartItemRequest request)
        {
            var cart = await _api.AddCartItem(cartId, request.MenuItemID, request.Quantity);
            return cart == null ? BadRequest() : Json(cart);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateItemApi(int cartItemId, [FromBody] CartItemUpdateRequest request)
        {
            var cart = await _api.UpdateCartItem(cartItemId, request.Quantity);
            return cart == null ? NotFound() : Json(cart);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveItemApi(int cartItemId)
        {
            var cart = await _api.RemoveCartItem(cartItemId);
            return cart == null ? NotFound() : Json(cart);
        }

        private async Task<ApiCart?> GetOrCreateCart(int customerId)
        {
            var cart = await _api.GetCustomerCart(customerId);
            return cart ?? await _api.CreateCart(customerId);
        }
    }
}

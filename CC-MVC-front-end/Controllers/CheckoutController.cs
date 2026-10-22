using Microsoft.AspNetCore.Mvc;
using CrustyCrab.Models;
using CrustyCrab.Services;

namespace CrustyCrab.Controllers
{
    public class CheckoutController : Controller
    {
        private static readonly Dictionary<string, decimal> AddonPrices = new()
        {
            { "Extra Jumbo Prawns (5pcs)", 45.00m },
            { "Extra Corn on the Cob (2pcs)", 25.00m }
        };

        private readonly CrustyApiClient _api;

        public CheckoutController(CrustyApiClient api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index(int? menuItemId, string? spiceLevel, List<string>? addons, bool fromCart = false)
        {
            var customerId = HttpContext.Session.GetInt32("UserId");
            var model = new CheckoutViewModel
            {
                SpiceLevel = string.IsNullOrWhiteSpace(spiceLevel) ? "Medium" : spiceLevel,
                Addons = addons ?? new List<string>(),
                FromCart = fromCart
            };

            ApiMenuItem? item = null;
            if (fromCart)
            {
                if (!customerId.HasValue)
                    return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Index), "Checkout", new { fromCart = true }) });

                model.Cart = await _api.GetCustomerCart(customerId.Value);
                if (model.Cart == null || !model.Cart.Items.Any())
                {
                    TempData["Error"] = "Your cart is empty.";
                    return RedirectToAction("Index", "Cart");
                }
                model.Subtotal = model.Cart.TotalAmount;
            }
            else
            {
                if (menuItemId.HasValue)
                    item = await _api.GetMenuItem(menuItemId.Value);

                if (item == null)
                {
                    var menu = await _api.GetMenu();
                    item = menu?.FirstOrDefault(m => m.Availability);
                }

                if (item != null)
                {
                    model.MenuItemId = item.MenuItemID;
                    model.Subtotal = item.Price + AddonTotal(model.Addons);
                }
            }

            if (customerId.HasValue)
            {
                model.Addresses = await _api.GetCustomerAddresses(customerId.Value) ?? new List<ApiDeliveryAddress>();
                model.PaymentMethods = await _api.GetPaymentMethods()
      ?? new List<ApiPaymentMethod>();

                model.SavedPaymentMethods = await _api.GetCustomerPaymentMethods(customerId.Value)
                    ?? new List<ApiCustomerPaymentMethod>();

                model.PaymentMethodId = model.SavedPaymentMethods.FirstOrDefault()?.PaymentMethodID
                    ?? model.PaymentMethods.FirstOrDefault()?.PaymentMethodID
                    ?? 0;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AuthorizePayment(CheckoutViewModel model)
        {
            if (model.FromCart)
            {
                var cartCustomerId = HttpContext.Session.GetInt32("UserId");
                if (!cartCustomerId.HasValue)
                    return RedirectToAction("Login", "Auth");

                model.Cart = await _api.GetCustomerCart(cartCustomerId.Value);
                if (model.Cart == null || !model.Cart.Items.Any())
                {
                    TempData["Error"] = "Your cart is empty.";
                    return RedirectToAction("Index", "Cart");
                }
            }

            ApiMenuItem? item = null;
            if (!model.FromCart)
            {
                item = await _api.GetMenuItem(model.MenuItemId);
                if (item == null || !item.Availability)
                {
                    TempData["Error"] = "That item is no longer available.";
                    return RedirectToAction(nameof(Index), new { menuItemId = model.MenuItemId, spiceLevel = model.SpiceLevel, addons = model.Addons });
                }
            }

            var customerId = HttpContext.Session.GetInt32("UserId");
            ApiCustomer? customer = null;
            if (customerId.HasValue && string.Equals(HttpContext.Session.GetString("Role"), "Customer", StringComparison.OrdinalIgnoreCase))
            {
                customer = await _api.GetCustomer(customerId.Value);
            }
            else
            {
                var nameParts = (model.CardholderName ?? string.Empty).Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                var firstName = nameParts.Length > 0 ? nameParts[0] : "Guest";
                var lastName = nameParts.Length > 1 ? nameParts[1] : "Customer";
                var guestKey = Guid.NewGuid().ToString("N");
                customer = await _api.CreateCustomer(firstName, lastName, "0000000000", $"guest-{guestKey}@crustycrab.local", guestKey);
            }

            if (customer == null)
            {
                TempData["Error"] = "We could not create your order. Please sign in and try again.";
                return RedirectToAction(nameof(Index), new { menuItemId = model.MenuItemId, spiceLevel = model.SpiceLevel, addons = model.Addons });
            }

            if (string.Equals(model.FulfillmentMethod, "Delivery", StringComparison.OrdinalIgnoreCase) && !model.DeliveryAddressId.HasValue)
            {
                TempData["Error"] = "Select a delivery address before placing a delivery order.";
                return RedirectToAction(nameof(Index), new { menuItemId = model.MenuItemId, spiceLevel = model.SpiceLevel, addons = model.Addons });
            }

            List<ApiOrderItemRequest> orderItems;
            if (model.FromCart && model.Cart != null)
            {
                orderItems = model.Cart.Items.Select(cartItem => new ApiOrderItemRequest
                {
                    MenuItemID = cartItem.MenuItemID,
                    Quantity = cartItem.Quantity
                }).ToList();
            }
            else
            {
                var notes = $"Spice: {model.SpiceLevel}";
                if (model.Addons.Count > 0)
                    notes += "; Add-ons: " + string.Join(", ", model.Addons);

                orderItems = new List<ApiOrderItemRequest>
                {
                    new ApiOrderItemRequest
                    {
                        MenuItemID = item!.MenuItemID,
                        Quantity = 1,
                        Notes = notes
                    }
                };
            }

            var order = await _api.CreateOrder(
                customer.CustomerID,
                model.DeliveryAddressId,
                model.FulfillmentMethod,
                orderItems);

            if (order == null)
            {
                TempData["Error"] = "We could not place your order. Please try again.";
                return RedirectToAction(nameof(Index), new { menuItemId = model.MenuItemId, spiceLevel = model.SpiceLevel, addons = model.Addons });
            }

            var methods = model.PaymentMethods.Count > 0
                ? model.PaymentMethods
                : await _api.GetPaymentMethods() ?? new List<ApiPaymentMethod>();
            var method = methods.FirstOrDefault(m => m.PaymentMethodID == model.PaymentMethodId)
                         ?? methods.FirstOrDefault(m => m.MethodName.Contains("Card", StringComparison.OrdinalIgnoreCase))
                         ?? methods.FirstOrDefault()
                         ?? await _api.CreatePaymentMethod("Card");

            if (method == null)
            {
                TempData["Error"] = "Payment is unavailable right now. Please try again.";
                return RedirectToAction(nameof(Index), new { menuItemId = model.MenuItemId, spiceLevel = model.SpiceLevel, addons = model.Addons });
            }

            var subtotal = model.FromCart && model.Cart != null
                ? model.Cart.TotalAmount
                : item!.Price + AddonTotal(model.Addons);
            var fee = model.FulfillmentMethod == "Delivery" ? 50.00m : 0.00m;
            var payment = await _api.CreatePayment(order.OrderID, method.PaymentMethodID, subtotal + fee);
            if (payment == null)
            {
                TempData["Error"] = "Your payment could not be processed. Please try again.";
                return RedirectToAction(nameof(Index), new { menuItemId = model.MenuItemId, spiceLevel = model.SpiceLevel, addons = model.Addons });
            }

            await _api.UpdatePaymentStatus(payment.PaymentID, "Paid");

            if (model.FromCart && model.Cart != null)
            {
                foreach (var cartItem in model.Cart.Items)
                    await _api.RemoveCartItem(cartItem.CartItemID);
            }

            return RedirectToAction("Review", "Menu");
        }

        private static decimal AddonTotal(IEnumerable<string>? addons)
        {
            return (addons ?? Enumerable.Empty<string>()).Sum(a => AddonPrices.TryGetValue(a, out var price) ? price : 0m);
        }
    }
}

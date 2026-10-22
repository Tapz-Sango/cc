using CrustyCrab.Models;
using CrustyCrab.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrustyCrab.Controllers
{
    public class CustomerController : Controller
    {
        private readonly CrustyApiClient _api;

        public CustomerController(CrustyApiClient api)
        {
            _api = api;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var customerId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");
            if (!customerId.HasValue || !string.Equals(role, "Customer", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Index), "Customer") });

            var customer = await _api.GetCustomer(customerId.Value);
            if (customer == null)
                return RedirectToAction("Logout", "Auth");

            var model = new AccountViewModel
            {
                Customer = customer,
                Addresses = await _api.GetCustomerAddresses(customerId.Value) ?? new List<ApiDeliveryAddress>(),
                Orders = await _api.GetCustomerOrders(customerId.Value) ?? new List<ApiOrder>(),
                SavedPaymentMethods = await _api.GetCustomerPaymentMethods(customerId.Value) ?? new List<ApiCustomerPaymentMethod>(),
                PaymentMethods = await _api.GetPaymentMethods() ?? new List<ApiPaymentMethod>()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var customer = await _api.GetCustomer(id);
            return customer == null ? NotFound() : Json(customer);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new CustomerRequest());
        }

        [HttpPost]
        public async Task<IActionResult> Register(CustomerRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var customer = await _api.CreateCustomer(request.FirstName, request.LastName, request.PhoneNumber, request.EmailAddress, request.Password);
            if (customer == null)
            {
                ModelState.AddModelError(string.Empty, "That email address may already be registered or the API is unavailable.");
                return View(request);
            }

            var login = await _api.CustomerLogin(request.EmailAddress, request.Password);
            if (login != null && login.Success)
            {
                HttpContext.Session.SetInt32("UserId", login.UserId);
                HttpContext.Session.SetString("Username", login.Username ?? request.EmailAddress);
                HttpContext.Session.SetString("Role", login.Role ?? "Customer");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(CustomerUpdateRequest request)
        {
            var customerId = HttpContext.Session.GetInt32("UserId");
            if (!customerId.HasValue)
                return RedirectToAction("Login", "Auth");

            var customer = await _api.UpdateCustomer(customerId.Value, request.FirstName, request.LastName, request.PhoneNumber, request.EmailAddress);
            TempData[customer == null ? "Error" : "Success"] = customer == null
                ? "Your profile could not be updated."
                : "Your profile has been updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            var customerId = HttpContext.Session.GetInt32("UserId");
            if (!customerId.HasValue)
                return RedirectToAction("Login", "Auth");

            var deleted = await _api.DeleteCustomer(customerId.Value);
            if (deleted)
            {
                HttpContext.Session.Clear();
                TempData["Success"] = "Your account has been deleted.";
                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = "Your account could not be deleted.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ApiList()
        {
            var customers = await _api.GetCustomers();
            return customers == null ? StatusCode(502) : Json(customers);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerRequest request)
        {
            var customer = await _api.CreateCustomer(request.FirstName, request.LastName, request.PhoneNumber, request.EmailAddress, request.Password);
            return customer == null ? BadRequest() : Json(customer);
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] CustomerUpdateRequest request)
        {
            var customer = await _api.UpdateCustomer(id, request.FirstName, request.LastName, request.PhoneNumber, request.EmailAddress);
            return customer == null ? BadRequest() : Json(customer);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteForm(int id)
        {
            if (!string.Equals(HttpContext.Session.GetString("Role"), "Admin", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Login", "Auth");

            await _api.DeleteCustomer(id);
            return RedirectToAction("Index", "AdminStaff");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            return await _api.DeleteCustomer(id) ? NoContent() : NotFound();
        }
    }
}

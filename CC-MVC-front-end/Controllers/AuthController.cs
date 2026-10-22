using CrustyCrab.Models;
using CrustyCrab.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrustyCrab.Controllers
{
    public class AuthController : Controller
    {
        private readonly CrustyApiClient _api;

        public AuthController(CrustyApiClient api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request, bool admin, string? returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(request.Identifier) || string.IsNullOrWhiteSpace(request.Password))
            {
                TempData["Error"] = "Enter your login details.";
                return RedirectToAction(nameof(Login), new { returnUrl });
            }

            var response = admin
                ? await _api.AdminLogin(request.Identifier, request.Password)
                : await _api.CustomerLogin(request.Identifier, request.Password);

            if (response == null || !response.Success)
            {
                TempData["Error"] = response?.Message ?? "The login details could not be verified.";
                return RedirectToAction(nameof(Login), new { returnUrl });
            }

            HttpContext.Session.SetInt32("UserId", response.UserId);
            HttpContext.Session.SetString("Username", response.Username ?? string.Empty);
            HttpContext.Session.SetString("Role", response.Role ?? (admin ? "Admin" : "Customer"));

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(admin ? "Portal" : "Index", admin ? "Manager" : "Customer");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> CustomerLogin([FromBody] LoginRequest request)
        {
            var response = await _api.CustomerLogin(request.Identifier, request.Password);
            return response == null ? StatusCode(502, new { message = "Authentication service is unavailable." }) : Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> AdminLogin([FromBody] LoginRequest request)
        {
            var response = await _api.AdminLogin(request.Identifier, request.Password);
            return response == null ? StatusCode(502, new { message = "Authentication service is unavailable." }) : Json(response);
        }
    }
}

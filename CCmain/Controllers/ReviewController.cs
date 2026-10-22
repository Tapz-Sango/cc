using CrustyCrab.Models;
using CrustyCrab.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrustyCrab.Controllers
{
    public class ReviewController : Controller
    {
        private readonly CrustyApiClient _api;

        public ReviewController(CrustyApiClient api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsCustomer())
                return RedirectToAction("Login", "Auth");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int rating, string comment)
        {
            if (!IsCustomer())
                return RedirectToAction("Login", "Auth");

            if (rating < 1 || rating > 5)
            {
                ModelState.AddModelError(nameof(rating), "Please select a rating from 1 to 5.");
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                ModelState.AddModelError(nameof(comment), "Please enter a comment.");
            }
            else if (comment.Trim().Length > 2000)
            {
                ModelState.AddModelError(nameof(comment), "Your comment cannot exceed 2000 characters.");
            }

            if (!ModelState.IsValid)
                return View();

            var customerId = HttpContext.Session.GetInt32("UserId")!.Value;

            var review = await _api.CreateReview(customerId, rating, comment.Trim());

            if (review == null)
            {
                TempData["Error"] = "Your review could not be submitted. Please try again.";
                return View();
            }

            TempData["Success"] = "Thank you! Your review has been submitted.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            var reviews = await _api.GetReviews();

            if (reviews == null)
            {
                TempData["Error"] = "Reviews could not be loaded.";
                reviews = new List<ApiCustomerReview>();
            }

            return View(reviews);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Respond(int reviewId, string adminResponse)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            if (string.IsNullOrWhiteSpace(adminResponse) || adminResponse.Trim().Length > 2000)
            {
                TempData["Error"] = "Enter a response of between 1 and 2000 characters.";
                return RedirectToAction(nameof(Manage));
            }

            var adminId = HttpContext.Session.GetInt32("UserId")!.Value;

            var result = await _api.RespondToReview(
                reviewId, adminId, adminResponse.Trim());

            TempData[result == null ? "Error" : "Success"] =
                result == null
                    ? "The response could not be saved."
                    : "Your response has been saved.";

            return RedirectToAction(nameof(Manage));
        }
        [HttpGet]
        public async Task<IActionResult> MyReviews()
        {
            if (!IsCustomer())
                return RedirectToAction("Login", "Auth");

            var customerId = HttpContext.Session.GetInt32("UserId")!.Value;
            var reviews = await _api.GetCustomerReviews(customerId);

            if (reviews == null)
            {
                TempData["Error"] = "Your reviews could not be loaded.";
                reviews = new List<ApiCustomerReview>();
            }

            return View(reviews);
        }

        private bool IsCustomer()
        {
            return HttpContext.Session.GetInt32("UserId").HasValue
                && string.Equals(
                    HttpContext.Session.GetString("Role"),
                    "Customer",
                    StringComparison.OrdinalIgnoreCase);
        }


        private bool IsAdmin()
        {
            return HttpContext.Session.GetInt32("UserId").HasValue
                && string.Equals(
                    HttpContext.Session.GetString("Role"),
                    "Admin",
                    StringComparison.OrdinalIgnoreCase);
        }
    }
}
using CrustyCrab.Models;
using CrustyCrab.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrustyCrab.Controllers
{
    public class DeliveryAddressController : Controller
    {
        private readonly CrustyApiClient _api;

        public DeliveryAddressController(CrustyApiClient api)
        {
            _api = api;
        }

        [HttpPost]
        public async Task<IActionResult> CreateForm(DeliveryAddressRequest request)
        {
            var customerId = HttpContext.Session.GetInt32("UserId");
            if (!customerId.HasValue)
                return RedirectToAction("Login", "Auth");

            var address = await _api.CreateAddress(customerId.Value, request.Suburb, request.HouseNumber, request.StreetName, request.BuildingType);
            TempData[address == null ? "Error" : "Success"] = address == null
                ? "The delivery address could not be added."
                : "Delivery address added.";
            return RedirectToAction("Index", "Customer");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateForm(DeliveryAddressUpdateRequest request, int id)
        {
            var address = await _api.UpdateAddress(id, request.Suburb, request.HouseNumber, request.StreetName, request.BuildingType);
            TempData[address == null ? "Error" : "Success"] = address == null
                ? "The delivery address could not be updated."
                : "Delivery address updated.";
            return RedirectToAction("Index", "Customer");
        }


        [HttpPost]
        public async Task<IActionResult> DeleteForm(int id)
        {
            var result = await _api.DeleteAddress(id);

            if (result.Success)
            {
                TempData["Success"] = "Delivery address removed.";
            }
            else
            {
                TempData["Error"] = result.ErrorMessage;
            }

            return RedirectToAction("Index", "Customer");
        }



        [HttpGet]
        public async Task<IActionResult> SearchAddressSuggestions([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 3)
                return Json(Array.Empty<AddressSuggestion>());

            var suggestions = await _api.SearchAddressSuggestions(q.Trim());

            return Json(suggestions ?? new List<AddressSuggestion>());
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var addresses = await _api.GetAddresses();
            return addresses == null ? StatusCode(502) : Json(addresses);
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var address = await _api.GetAddress(id);
            return address == null ? NotFound() : Json(address);
        }

        [HttpGet]
        public async Task<IActionResult> Customer(int customerId)
        {
            var addresses = await _api.GetCustomerAddresses(customerId);
            return addresses == null ? StatusCode(502) : Json(addresses);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DeliveryAddressRequest request)
        {
            var address = await _api.CreateAddress(request.CustomerID, request.Suburb, request.HouseNumber, request.StreetName, request.BuildingType);
            return address == null ? BadRequest() : Json(address);
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] DeliveryAddressUpdateRequest request)
        {
            var address = await _api.UpdateAddress(id, request.Suburb, request.HouseNumber, request.StreetName, request.BuildingType);
            return address == null ? NotFound() : Json(address);
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _api.DeleteAddress(id);

            if (result.Success)
            {
                return NoContent();
            }

            return Conflict(new
            {
                message = result.ErrorMessage ?? "The delivery address could not be deleted."
            });
        }

    }
}

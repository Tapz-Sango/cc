using CrustyCrab.Models;
using CrustyCrab.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrustyCrab.Controllers
{
    public class PaymentController : Controller
    {
        private readonly CrustyApiClient _api;

        public PaymentController(CrustyApiClient api)
        {
            _api = api;
        }

        [HttpPost]
        public async Task<IActionResult> SaveCustomerMethodForm(SavedPaymentMethodRequest request)
        {
            var customerId = HttpContext.Session.GetInt32("UserId");
            if (!customerId.HasValue)
                return RedirectToAction("Login", "Auth");

            var method = await _api.SaveCustomerPaymentMethod(customerId.Value, request.PaymentMethodID, request.CardholderName, request.CardNumber, request.ExpiryMonth, request.ExpiryYear);
            TempData[method == null ? "Error" : "Success"] = method == null
                ? "The payment method could not be saved."
                : "Payment method saved.";
            return RedirectToAction("Index", "Customer");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCustomerMethodForm(int id)
        {
            var customerId = HttpContext.Session.GetInt32("UserId");
            if (customerId.HasValue)
                await _api.DeleteCustomerPaymentMethod(id, customerId.Value);
            return RedirectToAction("Index", "Customer");
        }

        [HttpGet]
        public async Task<IActionResult> Methods()
        {
            var methods = await _api.GetPaymentMethods();
            return methods == null ? StatusCode(502) : Json(methods);
        }

        [HttpGet]
        public async Task<IActionResult> Method(int id)
        {
            var method = await _api.GetPaymentMethod(id);
            return method == null ? NotFound() : Json(method);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMethod([FromBody] PaymentMethodRequest request)
        {
            var method = await _api.CreatePaymentMethod(request.MethodName);
            return method == null ? BadRequest() : Json(method);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMethod(int id)
        {
            return await _api.DeletePaymentMethod(id) ? NoContent() : NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> CustomerMethods(int customerId)
        {
            var methods = await _api.GetCustomerPaymentMethods(customerId);
            return methods == null ? StatusCode(502) : Json(methods);
        }

        [HttpPost]
        public async Task<IActionResult> SaveCustomerMethod([FromBody] SavedPaymentMethodRequest request)
        {
            var method = await _api.SaveCustomerPaymentMethod(request.CustomerID, request.PaymentMethodID, request.CardholderName, request.CardNumber, request.ExpiryMonth, request.ExpiryYear);
            return method == null ? BadRequest() : Json(method);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCustomerMethod(int id, int customerId)
        {
            return await _api.DeleteCustomerPaymentMethod(id, customerId) ? NoContent() : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentRequest request)
        {
            var payment = await _api.CreatePayment(request.OrderID, request.PaymentMethodID, request.Amount);
            return payment == null ? BadRequest() : Json(payment);
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var payment = await _api.GetPayment(id);
            return payment == null ? NotFound() : Json(payment);
        }

        [HttpGet]
        public async Task<IActionResult> Order(int orderId)
        {
            var payments = await _api.GetOrderPayments(orderId);
            return payments == null ? StatusCode(502) : Json(payments);
        }

        [HttpPut]
        public async Task<IActionResult> Status(int id, [FromBody] PaymentStatusRequest request)
        {
            var payment = await _api.UpdatePaymentStatus(id, request.PaymentStatus);
            return payment == null ? NotFound() : Json(payment);
        }
    }
}

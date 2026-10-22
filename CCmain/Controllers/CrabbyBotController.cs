using CrustyCrab.Models;
using CrustyCrab.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrustyCrab.Controllers
{
    public class CrabbyBotController : Controller
    {
        private readonly CrustyApiClient _api;

        public CrabbyBotController(CrustyApiClient api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Respond([FromBody] CrabbyBotRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.OptionId))
                return BadRequest(new { message = "OptionId is required." });

            var response = await _api.GetCrabbyBotResponse(request.OptionId);
            return response == null ? StatusCode(502, new { message = "CrabbyBot is unavailable." }) : Json(response);
        }
    }
}

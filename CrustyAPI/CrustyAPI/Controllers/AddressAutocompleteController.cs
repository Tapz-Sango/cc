
using CrustyAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrustyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressAutocompleteController : ControllerBase
    {
        private readonly GeoapifyService _geoapifyService;
        private readonly ILogger<AddressAutocompleteController> _logger;

        public AddressAutocompleteController(
            GeoapifyService geoapifyService,
            ILogger<AddressAutocompleteController> logger)
        {
            _geoapifyService = geoapifyService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] string q,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 3)
            {
                return BadRequest(new
                {
                    message = "Enter at least three characters."
                });
            }

            if (q.Length > 200)
            {
                return BadRequest(new
                {
                    message = "The search is too long."
                });
            }

            try
            {
                var results = await _geoapifyService.SearchAsync(
                    q.Trim(),
                    cancellationToken);

                return Ok(results);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "Geoapify address search failed.");

                return StatusCode(502, new
                {
                    message = "Address search is temporarily unavailable."
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Geoapify configuration is missing.");

                return StatusCode(500, new
                {
                    message = "Address search is not configured."
                });
            }
        }
    }
}

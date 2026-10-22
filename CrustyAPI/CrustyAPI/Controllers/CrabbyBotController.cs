using CrustyAPI.DTOs;
using CrustyAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrustyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CrabbyBotController : ControllerBase
{
    private readonly CrabbyBotService _crabbyBotService;

    public CrabbyBotController(CrabbyBotService crabbyBotService)
    {
        _crabbyBotService = crabbyBotService;
    }

    [HttpPost]
    public ActionResult<CrabbybotResponse> GetResponse([FromBody] CrabbybotRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.OptionId))
        {
            return BadRequest("OptionId is required.");
        }

        var response = _crabbyBotService.GetResponse(request.OptionId);

        return Ok(response);
    }
}
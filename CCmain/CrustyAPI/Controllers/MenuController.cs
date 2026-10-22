using CrustyAPI.Models;
using CrustyAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrustyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly MenuService _menuService;
    private readonly IWebHostEnvironment _environment;

    public MenuController(
        MenuService menuService,
        IWebHostEnvironment environment)
    {
        _menuService = menuService;
        _environment = environment;
    }

    // GET: api/menu
    [HttpGet]
    public async Task<IActionResult> GetMenu()
    {
        var menu = await _menuService.GetMenu();

        return Ok(menu);
    }

    // POST: api menu
    [HttpPost]
    public async Task<IActionResult> CreateMenuItem(MenuItem menuItem)
    {
        var createdItem = await _menuService.CreateMenuItem(menuItem);

        return CreatedAtAction(
            nameof(GetMenuItem),
            new { id = createdItem.MenuItemID },
            createdItem
        );
    }

    // POST: api/menu/{id}/image
    [HttpPost("{id}/image")]
    public async Task<IActionResult> UploadImage(
        int id,
        IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            return BadRequest(new
            {
                message = "Please select an image file."
            });
        }

        var menuItem = await _menuService.GetMenuItemById(id);

        if (menuItem == null)
        {
            return NotFound(new
            {
                message = "Menu item not found."
            });
        }

        var allowedExtensions = new[]
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            return BadRequest(new
            {
                message = "Only JPG, JPEG, PNG and WEBP images are allowed."
            });
        }

        const long maxFileSize = 5 * 1024 * 1024;

        if (image.Length > maxFileSize)
        {
            return BadRequest(new
            {
                message = "The image must be 5 MB or smaller."
            });
        }

        var imagesPath = Path.Combine(
     _environment.ContentRootPath,
     "Images",
     "Menu");

        Directory.CreateDirectory(imagesPath);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(imagesPath, fileName);

        await using (var stream = new FileStream(
            filePath,
            FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        var imageUrl = $"/Images/Menu/{fileName}";

        menuItem.ImageUrl = imageUrl;

        await _menuService.UpdateMenuItem(id, menuItem);

        return Ok(menuItem);
    }

    // GET: api/menu/popular
    [HttpGet("popular")]
    public async Task<IActionResult> GetPopularMenuItems()
    {
        var menuItems = await _menuService.GetPopularMenuItems();

        return Ok(menuItems);
    }


    // GET: api/menu/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMenuItem(int id)
    {
        var menuItem = await _menuService.GetMenuItemById(id);

        if (menuItem == null)
        {
            return NotFound(new
            {
                message = "Menu item not found"
            });
        }

        return Ok(menuItem);
    }

    // PUT: api/menu/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMenuItem(
        int id,
        MenuItem menuItem)
    {
        var updatedItem =
            await _menuService.UpdateMenuItem(id, menuItem);

        if (updatedItem == null)
        {
            return NotFound(new
            {
                message = "Menu item not found"
            });
        }

        return Ok(updatedItem);
    }

    // DELETE: api/menu/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMenuItem(int id)
    {
        var result = await _menuService.DeleteMenuItem(id);

        if (result.Blocked)
        {
            return Conflict(new
            {
                message =
                    "This menu item cannot be deleted because it is used in an existing cart or order. Mark it as unavailable instead."
            });
        }

        if (!result.Success)
        {
            return NotFound(new
            {
                message = "Menu item not found"
            });
        }

        return NoContent();
    }
}
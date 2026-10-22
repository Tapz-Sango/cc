using CrustyAPI.Data;
using CrustyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CrustyAPI.Services;

public class MenuService
{
    private readonly AppDbContext _context;

    public MenuService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MenuItem>> GetMenu()
    {
        return await _context.MenuItems
            .ToListAsync();
    }

    public async Task<MenuItem?> GetMenuItemById(int id)
    {
        return await _context.MenuItems
            .FirstOrDefaultAsync(m => m.MenuItemID == id);
    }

    public async Task<MenuItem> CreateMenuItem(MenuItem menuItem)
    {
        _context.MenuItems.Add(menuItem);

        await _context.SaveChangesAsync();

        return menuItem;
    }

    public async Task<List<MenuItem>> GetPopularMenuItems()
    {
        return await _context.MenuItems
            .Where(m => m.IsPopular && m.Availability)
            .ToListAsync();
    }


    public async Task<MenuItem?> UpdateMenuItem(int id, MenuItem menuItem)
    {
        var existingItem = await _context.MenuItems
            .FirstOrDefaultAsync(m => m.MenuItemID == id);

        if (existingItem == null)
        {
            return null;
        }

        existingItem.ItemName = menuItem.ItemName;
        existingItem.Description = menuItem.Description;
        existingItem.Price = menuItem.Price;
        existingItem.Category = menuItem.Category;
        existingItem.Availability = menuItem.Availability;
        existingItem.ImageUrl = menuItem.ImageUrl;
        existingItem.IsPopular = menuItem.IsPopular;

        await _context.SaveChangesAsync();

        return existingItem;
    }
    public async Task<(bool Success, bool Blocked)> DeleteMenuItem(int id)
    {
        var menuItem = await _context.MenuItems
            .FirstOrDefaultAsync(m => m.MenuItemID == id);

        if (menuItem == null)
        {
            return (false, false);
        }

        _context.MenuItems.Remove(menuItem);

        try
        {
            await _context.SaveChangesAsync();
            return (true, false);
        }
        catch (DbUpdateException)
        {
            return (false, true);
        }
    }
}
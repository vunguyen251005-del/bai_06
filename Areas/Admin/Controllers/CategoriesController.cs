using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThuongMaiDienTu.Data;
using ThuongMaiDienTu.Models;

namespace ThuongMaiDienTu.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = DbInitializer.AdminRole)]
public class CategoriesController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var categories = await context.Categories
            .Include(c => c.Products)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return View(categories);
    }

    public IActionResult Create()
    {
        return View(new Category());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (!ModelState.IsValid)
        {
            return View(category);
        }

        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await context.Categories.FindAsync(id);
        return category is null ? NotFound() : View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category category)
    {
        if (id != category.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(category);
        }

        context.Categories.Update(category);
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var category = await context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        return category is null ? NotFound() : View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category is not null)
        {
            context.Products.RemoveRange(category.Products);
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ThuongMaiDienTu.Models;

namespace ThuongMaiDienTu.Data;

public static class DbInitializer
{
    public const string AdminRole = "Admin";
    public const string CustomerRole = "Customer";
    public const string AdminEmail = "admin@shop.local";
    public const string AdminPassword = "Admin@123";

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        await context.Database.MigrateAsync();

        foreach (var role in new[] { AdminRole, CustomerRole })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var admin = await userManager.FindByEmailAsync(AdminEmail);
        if (admin is null)
        {
            admin = new IdentityUser
            {
                UserName = AdminEmail,
                Email = AdminEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, AdminPassword);
        }

        if (!await userManager.IsInRoleAsync(admin, AdminRole))
        {
            await userManager.AddToRoleAsync(admin, AdminRole);
        }

        if (!await context.Categories.AnyAsync())
        {
            context.Categories.AddRange(
                new Category { Name = "Dien thoai" },
                new Category { Name = "Laptop" },
                new Category { Name = "Phu kien" });
            await context.SaveChangesAsync();
        }

        if (!await context.Products.AnyAsync())
        {
            var phone = await context.Categories.FirstAsync(c => c.Name == "Dien thoai");
            var laptop = await context.Categories.FirstAsync(c => c.Name == "Laptop");
            var accessory = await context.Categories.FirstAsync(c => c.Name == "Phu kien");

            context.Products.AddRange(
                new Product
                {
                    Name = "iPhone 15 Pro",
                    Price = 26990000,
                    Quantity = 15,
                    CategoryId = phone.Id,
                    ImageUrl = "https://images.unsplash.com/photo-1695048133142-1a20484d2569?auto=format&fit=crop&w=900&q=80",
                    Description = "Dien thoai cao cap, hieu nang manh, camera sac net."
                },
                new Product
                {
                    Name = "Laptop Ultrabook 14",
                    Price = 18990000,
                    Quantity = 9,
                    CategoryId = laptop.Id,
                    ImageUrl = "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?auto=format&fit=crop&w=900&q=80",
                    Description = "Laptop mong nhe cho hoc tap, lam viec va giai tri."
                },
                new Product
                {
                    Name = "Tai nghe Bluetooth",
                    Price = 1290000,
                    Quantity = 40,
                    CategoryId = accessory.Id,
                    ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&w=900&q=80",
                    Description = "Am thanh ro, pin lau, ket noi on dinh."
                });
            await context.SaveChangesAsync();
        }
    }
}

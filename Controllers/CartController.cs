using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThuongMaiDienTu.Extensions;
using ThuongMaiDienTu.Models;
using ThuongMaiDienTu.Repositories;

namespace ThuongMaiDienTu.Controllers;

[Authorize]
public class CartController(IProductRepository products) : Controller
{
    private const string CartKey = "SHOPPING_CART";

    public IActionResult Index()
    {
        return View(new ShoppingCartViewModel { Items = GetCart() });
    }

    public async Task<IActionResult> Add(int id)
    {
        var product = await products.GetByIdAsync(id);
        if (product is null)
        {
            return NotFound();
        }

        var cart = GetCart();
        var item = cart.FirstOrDefault(i => i.ProductId == id);
        if (item is null)
        {
            cart.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = 1,
                ImageUrl = product.ImageUrl
            });
        }
        else
        {
            item.Quantity++;
        }

        SaveCart(cart);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(int productId, int quantity)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(i => i.ProductId == productId);
        if (item is not null)
        {
            if (quantity <= 0)
            {
                cart.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }
        }

        SaveCart(cart);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Remove(int id)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(i => i.ProductId == id);
        if (item is not null)
        {
            cart.Remove(item);
            SaveCart(cart);
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Checkout()
    {
        var cart = GetCart();
        cart.Clear();
        SaveCart(cart);
        TempData["Message"] = "Dat hang thanh cong. Cam on ban da mua hang!";
        return RedirectToAction(nameof(Index));
    }

    private List<CartItem> GetCart()
    {
        return HttpContext.Session.GetObject<List<CartItem>>(CartKey) ?? new List<CartItem>();
    }

    private void SaveCart(List<CartItem> cart)
    {
        HttpContext.Session.SetObject(CartKey, cart);
    }
}

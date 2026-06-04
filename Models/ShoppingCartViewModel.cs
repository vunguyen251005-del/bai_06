namespace ThuongMaiDienTu.Models;

public class ShoppingCartViewModel
{
    public IList<CartItem> Items { get; set; } = new List<CartItem>();
    public decimal Total => Items.Sum(item => item.Total);
}

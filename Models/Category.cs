using System.ComponentModel.DataAnnotations;

namespace ThuongMaiDienTu.Models;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui long nhap ten danh muc")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}

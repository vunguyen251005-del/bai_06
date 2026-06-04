using System.ComponentModel.DataAnnotations;

namespace ThuongMaiDienTu.Models;

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui long nhap ten san pham")]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap gia")]
    [Range(0, 999999999, ErrorMessage = "Gia phai lon hon hoac bang 0")]
    public decimal Price { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [Display(Name = "Anh san pham")]
    [StringLength(500)]
    public string? ImageUrl { get; set; }

    [Range(0, 100000)]
    public int Quantity { get; set; }

    [Display(Name = "Danh muc")]
    public int CategoryId { get; set; }

    public Category? Category { get; set; }
}

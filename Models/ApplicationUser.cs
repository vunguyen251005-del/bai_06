using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ThuongMaiDienTu.Models;

public class ApplicationUser : IdentityUser
{
    [StringLength(150)]
    public string? FullName { get; set; }

    [StringLength(300)]
    public string? Address { get; set; }
}
